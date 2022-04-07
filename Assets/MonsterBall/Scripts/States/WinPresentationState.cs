using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using DG.Tweening;

public class WinPresentationState : State
{
    [SerializeField] private State EndGameState;
    [SerializeField] private Animator[] WinBackgrounds;
    [SerializeField] private WinDurationConfig WinConfig;
    [SerializeField] private AudioSource WinSource;

    public override void OnStateEnter()
    {
        if(Central.GlobalData.GameData.SpinWin > 0)
        {
            WinSource.clip = null;

            // Incrementation
            float incrementTime = 0f;
            int totalWin = Central.GlobalData.GameData.TotalWon * Central.GlobalData.BetMultiplier;
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

            IncrementerManager.Instance.WinMeter.Increment(totalWin, incrementTime);
            IncrementerManager.Instance.CreditMeter.Increment(Central.GlobalData.Money + totalWin, incrementTime, Central.GlobalData.Money);
            //Debug.LogError("Incrementing at " + incrementTime + " seconds");


            // Sound
            if (WinSource.clip != null && WinSource.clip.length >= .5f)
            {
                SoundManager.Instance.Fade(SoundConfig.Instance.BackgroundMusic, .25f, .5f);
            }

            SoundManager.Instance.PlayAndFade(WinSource, 1f, .2f, 0f);

            HighlightWinBackgrounds();
        }
    }

    public void HighlightWinBackgrounds()
    {
        int[] symbols = new int[3] { Central.GlobalData.GameData.ReelsResult[0][1], Central.GlobalData.GameData.ReelsResult[1][1], Central.GlobalData.GameData.ReelsResult[2][1] };
        
        for (int i = 0; i < symbols.Length; i++)
        {
            SymbolData checkSymbolData = Math.Instance.GetSymbolDataByID(symbols[i]);
            SymbolData winSymbolData = Math.Instance.GetSymbolDataByID(Central.GlobalData.GameData.WinDetail.SymbolID);
            if (symbols[i] == winSymbolData.SymbolID || winSymbolData.Type == SymbolType.MixedBar || checkSymbolData.Type == SymbolType.Wild)
            {
                string winAnimName = checkSymbolData.Name;
                if (checkSymbolData.Type == SymbolType.Wild)
                {
                    winAnimName = winSymbolData.Name;
                }

                WinBackgrounds[i].Play(winAnimName);
                SpriteRenderer winBackgroundSR = WinBackgrounds[i].GetComponent<SpriteRenderer>();
                winBackgroundSR.color = Color.clear;
                winBackgroundSR.DOColor(Color.white, .5f);
            }
        }
    }

    public override State OnUpdate()
    {
        State rtn = null;
        if(!IncrementerManager.Instance.Incrementing())
        {
            // Fix this to start on inc finish
            if(SoundConfig.Instance.BackgroundMusic.volume != .75f)
            {
                SoundManager.Instance.Fade(SoundConfig.Instance.BackgroundMusic, .75f, 1f);
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

    public void StopSymbolAnimations()
    {
        for (int i = 0; i < WinBackgrounds.Length; i++)
        {
            WinBackgrounds[i].Play("Idle");
            WinBackgrounds[i].GetComponent<SpriteRenderer>().color = Color.clear;
            WinBackgrounds[i].GetComponent<SpriteRenderer>().DOKill();
        }
    }
}
