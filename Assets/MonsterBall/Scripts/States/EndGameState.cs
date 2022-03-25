using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class EndGameState : State
{
    [SerializeField] private State IdleState;
    public override void OnStateEnter()
    {

    }

    public override State OnUpdate()
    {
        State rtn = null;
        rtn = IdleState;
        return rtn;
    }

    public override void OnStateExit()
    {

    }
}
