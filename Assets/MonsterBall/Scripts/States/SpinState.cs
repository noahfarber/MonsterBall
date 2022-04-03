using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class SpinState : State
{
    public ReelSpinController ReelsController;
    [SerializeField] private State PickGameState;
    [SerializeField] private State WinPresentationState;
    private bool _PlayButtonPressed = false;
    private int[] _DemoSymbols;

    public override void OnStateEnter()
    {
        _PlayButtonPressed = false;
        ReelsController.Spin();
    }

    public override State OnUpdate()
    {
        State rtn = null;
        
        if(_PlayButtonPressed) // Quick stop requested
        {
            ReelsController.RequestStop();
            _PlayButtonPressed = false;
        }

        // Transition check
        if (ReelsController.Spinning == false)
        {
            rtn = WinPresentationState;

            if (Central.GlobalData.GameData.WinDetail.SymbolID != -1)
            {
                if (Math.Instance.GetSymbolDataByID(Central.GlobalData.GameData.WinDetail.SymbolID).Type == SymbolType.Bonus)
                {
                    rtn = PickGameState;
                }
            }
        }

        return rtn;
    }

    public override void OnStateExit()
    {
        _DemoSymbols = null;
    }

    public void PlayButtonPressed()
    {
        _PlayButtonPressed = true;
    }
}
