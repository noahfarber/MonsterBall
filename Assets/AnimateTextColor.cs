using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class AnimateTextColor : MonoBehaviour
{
    private TextMeshProUGUI Text;
    [SerializeField] private Color _ColorA = Color.white;
    [SerializeField] private Color _ColorB = Color.black;
    [SerializeField] private float _Speed = .5f;

    // Start is called before the first frame update
    void Start()
    {
        Text = GetComponent<TextMeshProUGUI>();
        if(Text != null)
        {
            AnimateToColorA();
        }
    }

    void AnimateToColorA()
    {
        Text.DOColor(_ColorA, _Speed).OnComplete(AnimateToColorB);
    }

    void AnimateToColorB()
    {
        Text.DOColor(_ColorA, _Speed).OnComplete(AnimateToColorA);
    }
}
