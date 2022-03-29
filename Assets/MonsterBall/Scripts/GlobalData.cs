using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour
{
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
    IntProperty _TotalWon = new IntProperty();
    public IntProperty TotalWon
    {
        get { return _TotalWon; }
    }

    [SerializeField]
    IntProperty _SpinWin = new IntProperty();
    public IntProperty SpinWin
    {
        get { return _SpinWin; }
    }

    public WinDetail WinDetail = new WinDetail();

    public int[] ReelsEndPosition;
    public int[][] ReelsResult;
    public SymbolData[] SymbolInfo;

}

public class WinDetail
{
    public int SymbolID = -1;
    public int SymbolCount = -1;
    public PayModes PayMode = PayModes.None;
    public int Pay = 0;
}