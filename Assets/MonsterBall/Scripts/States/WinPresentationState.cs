using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class WinPresentationState : State
{
    [SerializeField] private State EndGameState;
    [SerializeField] private ParticleSystem[] ReelParticles;
    public override void OnStateEnter()
    {
        if(Central.GlobalData.GameData.TotalWon > 0)
        {
            float incrementTime = Central.GlobalData.GameData.TotalWon * .1f;
            IncrementerManager.Instance.WinMeter.Increment(Central.GlobalData.GameData.TotalWon, incrementTime);
            IncrementerManager.Instance.CreditMeter.Increment(Central.GlobalData.Money + Central.GlobalData.GameData.TotalWon, incrementTime, Central.GlobalData.Money);
        }
    }

    public override State OnUpdate()
    {
        State rtn = null;
        if(!IncrementerManager.Instance.Incrementing())
        {
            rtn = EndGameState;
        }
        return rtn;
    }

    public override void OnStateExit()
    {
        Central.GlobalData.Money.Value += Central.GlobalData.GameData.TotalWon; // Add win value to money
    }
}
