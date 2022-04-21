using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class MBPlayingState : NestedStateManager
    {
        public System.Action PlayButtonPressed;

        [SerializeField] private GameObject _Game;
        [SerializeField] private GameObject _GameUI;
        [SerializeField] private GameObject _Reels;
        [SerializeField] private int BaseBet = 5;
        [SerializeField] private int[] BetMultipliers;


        [SerializeField] private IdleState _IdleState;
        [SerializeField] private SpinState _SpinState;
        private SoundConfig _SoundConfig;

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            //ToggleGame();
            AddCallbacks();
        }

        public override State OnUpdate()
        {
            State rtn = null;

            if (!_SoundConfig.BackgroundMusic.isPlaying)
            {
                _SoundConfig.PlayNextBackgroundClip();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                PressPlayButton();
            }

            base.OnUpdate(); // Updated nested states

            return rtn;
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            //ToggleGame(false);
            RemoveCallbacks();
        }

        public void PressPlayButton()
        {
            PlayButtonPressed?.Invoke();
        }

        public void RequestBetChange(int direction)
        {
            for (int i = 0; i < BetMultipliers.Length; i++)
            {
                if (Central.GlobalData.BetMultiplier == BetMultipliers[i])
                {
                    Central.GlobalData.BetMultiplier.Value = BetMultipliers[(i + direction + BetMultipliers.Length) % BetMultipliers.Length];
                    Central.GlobalData.BetAmount.Value = BaseBet * Central.GlobalData.BetMultiplier.Value;
                    return;
                }
            }

            Debugger.Instance.LogError("Couldn't find bet in bet list.");
        }

        private void ToggleGame(bool enabled = true)
        {
            _Game.SetActive(enabled);
            _GameUI.SetActive(enabled);
            _Reels.SetActive(enabled);
        }

        private void AddCallbacks()
        {
            PlayButtonPressed += _IdleState.PlayButtonPressed;
            PlayButtonPressed += _SpinState.PlayButtonPressed;
            PlayButtonPressed += IncrementerManager.Instance.WinMeter.OnPlayButtonPressed;
            PlayButtonPressed += IncrementerManager.Instance.CreditMeter.OnPlayButtonPressed;

            if(_SoundConfig == null)
            {
                _SoundConfig = SoundConfig.Instance;
            }
        }

        private void RemoveCallbacks()
        {
            PlayButtonPressed -= _IdleState.PlayButtonPressed;
            PlayButtonPressed -= _SpinState.PlayButtonPressed;
            PlayButtonPressed -= IncrementerManager.Instance.WinMeter.OnPlayButtonPressed;
            PlayButtonPressed -= IncrementerManager.Instance.CreditMeter.OnPlayButtonPressed;
        }
    }
}