using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Incrementer : MonoBehaviour
{
    public TextMeshPro Text;
    public System.Action IncrementationComplete;

    private bool _Incrementing = false;
    private int _Value = 0;
    private int _FinalValue = 0;
    private float _Duration= 0f;
    private float _IncrementTimer = 0f;

    void Start()
    {
        if(Text == null && GetComponentInChildren<TextMeshPro>() != null)
        {
            Text = GetComponentInChildren<TextMeshPro>();
        }
     
        UpdateText();
    }

    void Update()
    {
        if (_Incrementing)
        {
            if(_IncrementTimer >= _Duration)
            {
                CompleteIncrementation();
            }
            else
            {
                _Value = (int)((_IncrementTimer / _Duration) * _FinalValue);
                _IncrementTimer += Time.deltaTime;
            }

            UpdateText();
        }
    }

    public void Increment(int toValue, float duration, int startValue = 0)
    {
        _Value = startValue;
        _Duration = duration;
        _FinalValue = toValue;
        _IncrementTimer = 0f;
        _Incrementing = true;
    }

    private void CompleteIncrementation()
    {
        _Value = _FinalValue;
        _Incrementing = false;
        IncrementationComplete?.Invoke();
    }

    private void UpdateText()
    {
        Text.text = _Value.ToString();
    }
}
