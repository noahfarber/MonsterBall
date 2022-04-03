using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using DG.Tweening;

public class PickGameIntro : State
{
    [SerializeField] private PickGameState _PickGameState;
    [SerializeField] private State _PickingState;

    public SpriteRenderer BlackFilter;
    public GameObject PickBackground;
    public GameObject PickTiles;
    private bool IntroDone = false;

    public override void OnStateEnter()
    {
        IntroDone = false;
        _PickGameState.PickGameView.transform.DOScaleX(1f, 1f).SetEase(Ease.InOutBounce).OnComplete(ViewSet);
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

    private void ViewSet()
    {
        IntroDone = true;
    }

}
