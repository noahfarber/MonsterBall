using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class WinPresentationState : State
{
    [SerializeField] private State EndGameState;
    [SerializeField] private ParticleSystem[] ReelParticles;
    [SerializeField] private WinDurationConfig WinConfig;
    [SerializeField] private AudioSource WinSource;

    public override void OnStateEnter()
    {
        if(Central.GlobalData.GameData.SpinWin > 0)
        {
            WinSource.clip = null;

            float incrementTime = 0f;
            for (int i = 0; i < WinConfig.SoundConfig.Length; i++)
            {
                if(((float)Central.GlobalData.GameData.WinDetail.Pay / 5f) < WinConfig.SoundConfig[i].MaxBetMultiple)
                {
                    WinSoundDetail detail = WinConfig.SoundConfig[i];
                    incrementTime = detail.Duration;
                    if(detail.Clip != null)
                    {
                        WinSource.clip = WinConfig.SoundConfig[i].Clip;
                    }
                    break;
                }
            }

            //Debug.LogError("Incrementing at " + incrementTime + " seconds");

            int spinWin = Central.GlobalData.GameData.SpinWin * Central.GlobalData.BetMultiplier;
            IncrementerManager.Instance.WinMeter.Increment(spinWin, incrementTime);
            IncrementerManager.Instance.CreditMeter.Increment(Central.GlobalData.Money + spinWin, incrementTime, Central.GlobalData.Money);
            
            if(WinSource.clip != null && WinSource.clip.length >= .5f)
            {
                SoundManager.Instance.Fade(SoundConfig.Instance.BackgroundMusic, .25f, .5f);
            }

            SoundManager.Instance.PlayAndFade(WinSource, 1f, .2f, 0f);
        }
    }

    public override State OnUpdate()
    {
        State rtn = null;
        if(!IncrementerManager.Instance.Incrementing())
        {
            if(SoundConfig.Instance.BackgroundMusic.volume != 1f)
            {
                SoundManager.Instance.Fade(SoundConfig.Instance.BackgroundMusic, 1f, 1f);
            }

            rtn = EndGameState;
        }
        return rtn;
    }

    public override void OnStateExit()
    {
        Central.GlobalData.Money.Value += Central.GlobalData.GameData.TotalWon * Central.GlobalData.BetMultiplier; // Add win value to money
        SoundManager.Instance.Fade(WinSource, 0f, .25f);
    }
}
