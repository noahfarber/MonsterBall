using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class SpinState : State
{
    public ReelSpinController ReelsController;
    [SerializeField] private State WinPresentationState;
    private bool _PlayButtonPressed = false;
    private int[] _DemoSymbols;

    public override void OnStateEnter()
    {
        _PlayButtonPressed = false;
        if (_DemoSymbols != null)
        {
            ReelsController.Spin(_DemoSymbols);
        }
        else
        {
            DazzleSpinData spinData = Central.MathGenerator.RequestOutcome().GetSpin(0);
            Central.GlobalData.TotalWon.Value = spinData.spinAward * Central.GlobalData.BetMultiplier;

            if (spinData.spinAward < spinData.totalAward)
            {
                Debugger.Instance.LogError("Missed a bonus");
            }

            ReelsController.Spin(spinData);
        }
    }

    public override State OnUpdate()
    {
        State rtn = null;
        
        if(_PlayButtonPressed)
        {
            ReelsController.RequestStop();
            _PlayButtonPressed = false;
        }  

        if(ReelsController.Spinning == false)
        {
            rtn = WinPresentationState;
        }

        return rtn;
    }

    public override void OnStateExit()
    {
        _DemoSymbols = null;
    }

    public void SetDemoSymbols(int[] symbols)
    {
        _DemoSymbols = symbols;
    }

    public void PlayButtonPressed()
    {
        _PlayButtonPressed = true;
    }
}
