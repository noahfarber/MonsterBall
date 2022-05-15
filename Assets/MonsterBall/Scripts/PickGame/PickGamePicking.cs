using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using TMPro;

public class PickGamePicking : State
{
    [SerializeField] private PickGameState _PickGameState;
    [SerializeField] private State _PickRecapState;
    [SerializeField] private float TimeToWaitAfterLastBomb = .75f;

    private float WaitAfterLastBombTimer = 0f;
    private int NumPicked = 0;

    public override void OnStateEnter()
    {
        NumPicked = 0;
        WaitAfterLastBombTimer = 0f;
    }

    public override State OnUpdate()
    {
        State rtn = null;

        if(NumPicked == _PickGameState.PickScript.Count)
        {
            if(WaitAfterLastBombTimer >= TimeToWaitAfterLastBomb)
            {
                rtn = _PickRecapState;
            }
            else
            {
                WaitAfterLastBombTimer += Time.deltaTime;
            }
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
            int amountWon = _PickGameState.PickScript[NumPicked];
            objectPicked.Clicked(amountWon);
            if(amountWon > 0)
            {
                AddPickValue(amountWon);
            }
            NumPicked++;
        }
    }

    private void AddPickValue(int value)
    {
        _PickGameState.CurrentPickWinAmount += value;
        _PickGameState.TotalWinMeter.Increment(_PickGameState.CurrentPickWinAmount, 1f, _PickGameState.TotalWinMeter.Value);
    }
}
