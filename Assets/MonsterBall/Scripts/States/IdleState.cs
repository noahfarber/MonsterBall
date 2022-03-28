using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class IdleState : State
{
    [SerializeField] private SpinState SpinState;

    public override void OnStateEnter()
    {

    }

    public override State OnUpdate()
    {
        State rtn = null;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rtn = SpinState;
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
}
