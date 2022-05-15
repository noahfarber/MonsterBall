using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IncrementerUI : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public System.Action IncrementationComplete;
    [HideInInspector] public bool Incrementing = false;

    public int Value
    {
        get
        {
            return _Value;
        }
    }

    private int _InitialValue = 0;
    private int _Value = 0;
    private int _DeltaValue = 0;
    private int _FinalValue = 0;
    private float _Duration = 0f;
    private float _IncrementTimer = 0f;

    void Start()
    {
        if (Text == null && GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            Text = GetComponentInChildren<TextMeshProUGUI>();
        }

        UpdateText();
    }

    void Update()
    {
        if (Incrementing)
        {
            if (_IncrementTimer >= _Duration)
            {
                CompleteIncrementation();
            }
            else
            {
                _Value = _InitialValue + (int)((_IncrementTimer / _Duration) * _DeltaValue);
                _IncrementTimer += Time.deltaTime;
            }

            UpdateText();
        }
    }

    public void Increment(int toValue, float duration, int startValue = 0)
    {
        _InitialValue = startValue;
        _Duration = duration;
        _DeltaValue = toValue - startValue;
        _FinalValue = toValue;
        _IncrementTimer = 0f;
        Incrementing = true;
    }

    public void SetValue(int value)
    {
        _Value = value;
        UpdateText();
    }

    public void OnPlayButtonPressed()
    {
        if(Incrementing)
        {
            _IncrementTimer = _Duration;
        }
    }

    private void CompleteIncrementation()
    {
        _Value = _FinalValue;
        Incrementing = false;
        IncrementationComplete?.Invoke();
    }

    private void UpdateText()
    {
        Text.text = _Value.ToString();
    }
}
