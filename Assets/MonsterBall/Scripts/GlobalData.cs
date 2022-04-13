using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour
{
    private void Start()
    {
        Load();    
    }

    [SerializeField]
    IntProperty _Money = new IntProperty();
    public IntProperty Money
    {
        get { return _Money; }
    }

    [SerializeField]
    IntProperty _BetMultiplier = new IntProperty();
    public IntProperty BetMultiplier
    {
        get { return _BetMultiplier; }
    }

    [SerializeField]
    IntProperty _BetAmount = new IntProperty();
    public IntProperty BetAmount
    {
        get { return _BetAmount; }
    }

    [SerializeField]
    GameData _GameData = new GameData();
    public GameData GameData
    {
        get { return _GameData; }
    }

    public void Load()
    {
        //Debug.LogError("Loading Money: " + PlayerPrefs.GetInt("Money", 200));
        _Money.Value = PlayerPrefs.GetInt("Money", 200);
        _BetMultiplier.Value = PlayerPrefs.GetInt("BetMultiplier", 1);
        _BetAmount.Value = PlayerPrefs.GetInt("BetAmount", 5);

        if (_GameData != null)
        {
            if (_GameData.LastOutCome != null)
            {
                PlayerPrefs.GetString("LastOutcome");
            }

            if (_GameData.CurrentSpin != null)
            {
                _GameData.CurrentSpin.syms = PlayerPrefs.GetString("CurrentSpinSyms");
                _GameData.CurrentSpin.stops = PlayerPrefs.GetString("CurrentSpinStops");
                _GameData.CurrentSpin.totalAward = PlayerPrefs.GetInt("CurrentSpinTotalAward");
            }

            _GameData.SpinWin.Value = PlayerPrefs.GetInt("SpinWin");
            _GameData.TotalWon.Value = PlayerPrefs.GetInt("TotalWon");

            if (_GameData.WinDetail != null)
            {
                _GameData.WinDetail.SymbolID = PlayerPrefs.GetInt("WinDetailSymbol");
                _GameData.WinDetail.SymbolCount = PlayerPrefs.GetInt("WinDetailSymbolCount");
                _GameData.WinDetail.PayMode = (PayModes)PlayerPrefs.GetInt("WinDetailPayMode");
                _GameData.WinDetail.SymbolID = PlayerPrefs.GetInt("WinDetailPay");
            }

            if (ReelSpinController.Instance != null && ReelSpinController.Instance.ReelsEndPosition != null && ReelSpinController.Instance.ReelsEndPosition.Length > 0)
            {
                for (int i = 0; i < ReelSpinController.Instance.ReelsEndPosition.Length; i++)
                {
                    ReelSpinController.Instance.ReelsEndPosition[i] = PlayerPrefs.GetInt("ReelsEndPos" + i.ToString(), ReelSpinController.Instance.ReelsEndPosition[i]);
                }
            }
        }
    }

    public void Save()
    {
        //Debug.LogError("Saving Money: " + _Money);
        PlayerPrefs.SetInt("Money", _Money);
        PlayerPrefs.SetInt("BetMultiplier", _BetMultiplier);
        PlayerPrefs.SetInt("BetAmount", _BetAmount);

        if(_GameData != null)
        {
            if (_GameData.LastOutCome != null)
            {
                PlayerPrefs.SetString("LastOutcome", _GameData.LastOutCome.ToString());
            }

            if (_GameData.CurrentSpin != null)
            {
                PlayerPrefs.SetString("CurrentSpinSyms", _GameData.CurrentSpin.syms);
                PlayerPrefs.SetString("CurrentSpinStops", _GameData.CurrentSpin.stops);
                PlayerPrefs.SetInt("CurrentSpinTotalAward", _GameData.CurrentSpin.totalAward);
            }

            PlayerPrefs.SetInt("SpinWin", _GameData.SpinWin);
            PlayerPrefs.SetInt("TotalWon", _GameData.TotalWon);

            if(_GameData.WinDetail != null)
            {
                PlayerPrefs.SetInt("WinDetailSymbol", _GameData.WinDetail.SymbolID);
                PlayerPrefs.SetInt("WinDetailSymbolCount", _GameData.WinDetail.SymbolCount);
                PlayerPrefs.SetInt("WinDetailPayMode", (int)_GameData.WinDetail.PayMode);
                PlayerPrefs.SetInt("WinDetailPay", (int)_GameData.WinDetail.Pay);
            }

            if(ReelSpinController.Instance != null && ReelSpinController.Instance.ReelsEndPosition != null && ReelSpinController.Instance.ReelsEndPosition.Length > 0)
            {
                for (int i = 0; i < ReelSpinController.Instance.ReelsEndPosition.Length; i++)
                {
                    PlayerPrefs.SetInt("ReelsEndPos" + i.ToString(), ReelSpinController.Instance.ReelsEndPosition[i]);
                }
            }
        }
    }

}

public class GameData
{
    [SerializeField]
    DazzleOutcome _LastOutCome = new DazzleOutcome();
    public DazzleOutcome LastOutCome
    {
        get { return _LastOutCome; }
        set { _LastOutCome = value; }
    }

    [SerializeField]
    DazzleSpinData _CurrentSpin = new DazzleSpinData();
    public DazzleSpinData CurrentSpin
    {
        get { return _CurrentSpin; }
        set { _CurrentSpin = value; }
    }

    [SerializeField]
    IntProperty _SpinWin = new IntProperty();
    public IntProperty SpinWin
    {
        get { return _SpinWin; }
    }

    [SerializeField]
    IntProperty _TotalWon = new IntProperty();
    public IntProperty TotalWon
    {
        get { return _TotalWon; }
    }

    public WinDetail WinDetail = new WinDetail();

    public int[][] ReelsResult;
}

public class WinDetail
{
    public int SymbolID = -1;
    public int SymbolCount = -1;
    public PayModes PayMode = PayModes.None;
    public int Pay = 0;
}