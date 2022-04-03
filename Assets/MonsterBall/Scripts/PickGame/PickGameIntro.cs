using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using DG.Tweening;

public class PickGameIntro : State
{
    [SerializeField] private PickGameState _PickGameState;
    [SerializeField] private State _PickingState;
    [SerializeField] private PickGameView View;

    private bool IntroDone = false;

    public override void OnStateEnter()
    {
        IntroDone = false;
        View.BonusWinText.transform.DOScale(1f, 4f).SetEase(Ease.InOutCubic).OnComplete(BonusTextFulLSize);

        if(View.BonusWinParticle != null)
        {
            View.BonusWinParticle.Play();
        }
    }

    public override State OnUpdate()
    {
        State rtn = null;

        if(IntroDone)
        {
            rtn = _PickingState;
        }

        return rtn;
    }

    public override void OnStateExit()
    {

    }

    private void BonusTextFulLSize()
    {
        View.BlackFilter.DOColor(Color.black, .75f).SetEase(Ease.InOutCubic).OnComplete(BlackFilterEnabled);
    }

    private void BlackFilterEnabled()
    {
        View.Toggle(true);
        View.BonusWinText.transform.localScale = Vector3.zero;
        View.BlackFilter.DOKill();
        View.BlackFilter.DOColor(Color.clear, 1.5f).SetEase(Ease.OutCubic).OnComplete(BlackFilterDisabled);
    }

    private void BlackFilterDisabled()
    {
        IntroDone = true;
    }

}
