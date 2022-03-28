using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class SpinState : State
{
    public ReelSpinController ReelsController;
    [SerializeField] private State WinPresentationState;
    private int[] _DemoSymbols;

    public override void OnStateEnter()
    {
        if(_DemoSymbols != null)
        {
            ReelsController.RequestSpin(_DemoSymbols);
        }
        else
        {
            ReelsController.RequestSpin();
        }
    }

    public override State OnUpdate()
    {
        State rtn = null;
        
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
}
