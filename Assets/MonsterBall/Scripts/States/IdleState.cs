using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class IdleState : State
{
    [SerializeField] private SpinState SpinState;
    [SerializeField] private WinPresentationState WinState;
    [SerializeField] private Animator SpinButton;

    private bool _TryStartGame = false;
    public override void OnStateEnter()
    {
        _TryStartGame = false;
        SpinButton.Play("Idle");
    }

    public override State OnUpdate()
    {
        State rtn = null;

        if (_TryStartGame)
        {
            if (Central.GlobalData.Money >= Central.GlobalData.BetAmount)
            {
                rtn = StartSpin();
            }

            _TryStartGame = false;
        }

        if(InputBehaviorTypes.GetKeyDown(KeyCode.KeypadPeriod))
        {
            rtn = GafSpin(new int[3] { 10, 10, 10 });
        }
        if (InputBehaviorTypes.GetKeyDown(KeyCode.Keypad0))
        {
            rtn = GafSpin(new int[3] { 0, 0, 0 });
        }
        if (InputBehaviorTypes.GetKeyDown(KeyCode.Keypad1))
        {
            rtn = GafSpin(new int[3] { 1, 1, 1 });
        }
        if (InputBehaviorTypes.GetKeyDown(KeyCode.Keypad2))
        {
            rtn = GafSpin(new int[3] { 2, 2, 2 });
        }
        if (InputBehaviorTypes.GetKeyDown(KeyCode.Keypad3))
        {
            rtn = GafSpin(new int[3] { 3, 3, 3 });
        }
        if (InputBehaviorTypes.GetKeyDown(KeyCode.Keypad4))
        {
            rtn = GafSpin(new int[3] { 4, 4, 4 });
        }
        if (InputBehaviorTypes.GetKeyDown(KeyCode.Keypad5))
        {
            rtn = GafSpin(new int[3] { 5, 5, 5 });
        }
        if (InputBehaviorTypes.GetKeyDown(KeyCode.Keypad6))
        {
            rtn = GafSpin(new int[3] { 6, 6, 6 });
        }
        if (InputBehaviorTypes.GetKeyDown(KeyCode.Keypad7))
        {
            rtn = GafSpin(new int[3] { 7, 7, 7 });
        }
        if (InputBehaviorTypes.GetKeyDown(KeyCode.Keypad8))
        {
            rtn = GafSpin(new int[3] { 8, 8, 8 });
        }

        return rtn;
    }

    public override void OnStateExit()
    {

    }

    public void PlayButtonPressed()
    {
        _TryStartGame = true;
    }


    private State GafSpin(int[] demoSymbols)
    {
        IncrementerManager.Instance.WinMeter.SetValue(0); // Clear Win Meter
        WinState.StopSymbolAnimations();
        Math.Instance.GenerateOutcome(demoSymbols);
        return SpinState;
    }

    private State StartSpin()
    {
        IncrementerManager.Instance.WinMeter.SetValue(0); // Clear Win Meter
        WinState.StopSymbolAnimations();
        Math.Instance.GenerateOutcome();
        Central.GlobalData.Money.Value -= Central.GlobalData.BetAmount; // Subtract Money
        SpinButton.Play("Spin");
        
        return SpinState;
    }

}

public class WinAnalyzer
{
    
}