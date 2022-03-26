using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class IdleState : State
{
    [SerializeField] private SpinState SpinState;
    private bool _TryStartGame = false;
    public override void OnStateEnter()
    {
        _TryStartGame = false;
    }

    public override State OnUpdate()
    {
        State rtn = null;

        if (_TryStartGame)
        {
            if (Central.GlobalData.Money > Central.GlobalData.BetAmount)
            {
                Central.GlobalData.Money.Value -= Central.GlobalData.BetAmount;
                IncrementerManager.Instance.WinMeter.SetValue(0);
                rtn = SpinState;
            }

            _TryStartGame = false;
        }

        if(Input.GetKeyDown(KeyCode.KeypadPeriod))
        {
            SpinState.SetDemoSymbols(new int[3] { 0, 0, 0 });
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            SpinState.SetDemoSymbols(new int[3] { 0, 0, 0 });
            rtn = SpinState;
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            SpinState.SetDemoSymbols(new int[3] { 1, 1, 1 });
            rtn = SpinState;
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            SpinState.SetDemoSymbols(new int[3] { 2, 2, 2 });
            rtn = SpinState;
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            SpinState.SetDemoSymbols(new int[3] { 3, 3, 3 });
            rtn = SpinState;
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            SpinState.SetDemoSymbols(new int[3] { 4, 4, 4 });
            rtn = SpinState;
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            SpinState.SetDemoSymbols(new int[3] { 5, 5, 5 });
            rtn = SpinState;
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

}
