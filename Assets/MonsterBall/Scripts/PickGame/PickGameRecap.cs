using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using DG.Tweening;

public class PickGameRecap : State
{
    [SerializeField] private PickGameState PickState;
    [SerializeField] private PickGameView View;
    private bool OutroDone = false;

    public override void OnStateEnter()
    {
        OutroDone = false;
        View.BlackFilter.DOColor(Color.black, .75f).SetEase(Ease.InOutCubic).OnComplete(BlackFilterEnabled);
    }

    public override State OnUpdate()
    {
        State rtn = null;

        if(OutroDone)
        {
            PickState.GameComplete();
        }

        return rtn;
    }

    public override void OnStateExit()
    {

    }

    private void BlackFilterEnabled()
    {
        View.Toggle(false);
        View.BlackFilter.DOKill();
        View.BlackFilter.DOColor(Color.clear, 1.5f).SetEase(Ease.OutCubic).OnComplete(BlackFilterDisabled);
    }

    private void BlackFilterDisabled()
    {
        OutroDone = true;
    }
}
