using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class WinPresentationState : State
{
    [SerializeField] private State EndGameState;
    [SerializeField] private IncrementerUI WinsIncrementer;
    [SerializeField] private IncrementerUI CreditsIncrementer;

    public override void OnStateEnter()
    {
        if(Central.GlobalData.TotalWon > 0)
        {
            float incrementTime = Central.GlobalData.TotalWon * .1f;
            WinsIncrementer.Increment(Central.GlobalData.TotalWon, incrementTime);
            CreditsIncrementer.Increment(Central.GlobalData.Money + Central.GlobalData.TotalWon, incrementTime, Central.GlobalData.Money);
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
        Central.GlobalData.Money.Value += Central.GlobalData.TotalWon;
    }
}
