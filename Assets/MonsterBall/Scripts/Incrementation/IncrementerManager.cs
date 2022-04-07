using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IncrementerManager : MonoBehaviour
{
    public static IncrementerManager Instance;
    public IncrementerUI WinMeter;
    public IncrementerUI CreditMeter;
    public TextMeshProUGUI BetText;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public bool Incrementing()
    {
        return WinMeter.Incrementing || CreditMeter.Incrementing;
    }

    private void Start()
    {
        Central.GlobalData.Money.OnPropertyChanged += OnPlayerMoneyChanged;
        Central.GlobalData.BetAmount.OnPropertyChanged += OnBetChanged;
        OnPlayerMoneyChanged();
        OnBetChanged();
    }

    private void OnApplicationQuit()
    {
        Central.GlobalData.Money.OnPropertyChanged -= OnPlayerMoneyChanged;
        Central.GlobalData.BetAmount.OnPropertyChanged -= OnBetChanged;
    }

    void OnPlayerMoneyChanged()
    {
        CreditMeter.SetValue(Central.GlobalData.Money);
    }

    void OnBetChanged()
    {
        BetText.text = Central.GlobalData.BetAmount.ToString();
    }
}
