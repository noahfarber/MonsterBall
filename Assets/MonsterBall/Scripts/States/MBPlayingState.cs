using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class MBPlayingState : NestedStateManager
    {
        public System.Action PlayButtonPressed;

        [SerializeField] private int BaseBet = 5;
        [SerializeField] private int[] BetMultipliers;

        [SerializeField] private GameObject _Game;

        [SerializeField] private IdleState _IdleState;
        [SerializeField] private SpinState _SpinState;

        [Header("Transition States")]
        [SerializeField] private State _Paused;
        private State _StateBeforePause;

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            _Game.SetActive(true);
            AddCallbacks();
        }

        public override State OnUpdate()
        {
            State rtn = null;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseGame();
            }

            if(Input.GetKeyDown(KeyCode.Space))
            {
                PressPlayButton();
            }

            base.OnUpdate(); // Updated nested states

            return rtn;
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            _Game.SetActive(false);
            RemoveCallbacks();
        }

        public void PauseGame()
        {
            if (CurrentState != _Paused)
            {
                _StateBeforePause = CurrentState;
                StateChange(_Paused);
            }
            else if (CurrentState == _Paused)
            {
                if (_StateBeforePause != null) { StateChange(_StateBeforePause); }
                else { Debugger.Instance.LogError("Can't unpause... No previous state found."); }
            }
        }

        public void PressPlayButton()
        {
            PlayButtonPressed?.Invoke();
        }

        public void RequestBetChange()
        {
            for (int i = 0; i < BetMultipliers.Length; i++)
            {
                if(Central.GlobalData.BetMultiplier == BetMultipliers[i])
                {
                    Central.GlobalData.BetMultiplier.Value = BetMultipliers[(i + 1) % BetMultipliers.Length];
                    Central.GlobalData.BetAmount.Value = BaseBet * Central.GlobalData.BetMultiplier.Value;
                    return;
                }
            }

            Debugger.Instance.LogError("Couldn't find bet in bet list.");
        }

        private void AddCallbacks()
        {
            PlayButtonPressed += _IdleState.PlayButtonPressed;
            PlayButtonPressed += _SpinState.PlayButtonPressed;
            PlayButtonPressed += IncrementerManager.Instance.WinMeter.OnPlayButtonPressed;
            PlayButtonPressed += IncrementerManager.Instance.CreditMeter.OnPlayButtonPressed;
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