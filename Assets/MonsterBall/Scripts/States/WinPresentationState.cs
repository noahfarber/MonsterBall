using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class WinPresentationState : State
{
    [SerializeField] private State EndGameState;

    public override void OnStateEnter()
    {

    }

    public override State OnUpdate()
    {
        State rtn = null;
        rtn = EndGameState;
        return rtn;
    }

    public override void OnStateExit()
    {

    }
}
