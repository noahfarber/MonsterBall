using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using DG.Tweening;

public class PickGameRecap : State
{
    [SerializeField] private PickGameState PickState;

    private bool ReadyToExit = false;

    public override void OnStateEnter()
    {
        ReadyToExit = false;
        PickState.PickGameView.transform.DOScaleX(0f, 1f).SetEase(Ease.InOutBounce).OnComplete(ViewSet);
    }

    public override State OnUpdate()
    {
        State rtn = null;

        if(ReadyToExit)
        {
            PickState.GameComplete();
        }

        return rtn;
    }

    public override void OnStateExit()
    {

    }

    private void ViewSet()
    {
        ReadyToExit = true;
    }
}
