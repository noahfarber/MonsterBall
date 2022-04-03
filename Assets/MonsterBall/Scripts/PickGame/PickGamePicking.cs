using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class PickGamePicking : State
{
    [SerializeField] private PickGameState _PickGameState;
    [SerializeField] private State _PickRecapState;

    private int NumPicked = 0;

    public override void OnStateEnter()
    {
        NumPicked = 0;
    }

    public override State OnUpdate()
    {
        State rtn = null;

        if(NumPicked == _PickGameState.PickScript.Count)
        {
            rtn = _PickRecapState;
        }

        return rtn;
    }

    public override void OnStateExit()
    {

    }

    public void PickObjectClicked(PickObject objectPicked)
    {
        if(NumPicked < _PickGameState.PickScript.Count && !objectPicked.Open)
        {
            objectPicked.Clicked(_PickGameState.PickScript[NumPicked]);
            NumPicked++;
        }
    }
}
