using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class Math : MonoBehaviour
{
    public static Math Instance;
    public ReelSpinController ReelsController;
    public ReelStrip[] ReelStrips;
    public SymbolData[] SymbolInfo;
    private int ReelCount = 3;
    private int[] _DemoSymbols;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }    
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateOutcome(int[] demoSymbols = null)
    {
        DazzleOutcome outcome = Central.MathGenerator.RequestOutcome();
        DazzleSpinData spinData = outcome.GetSpin(0);

        if (spinData.spinAward < spinData.totalAward)
        {
            Debugger.Instance.LogError("Missed a bonus award"); // Add code for bonus awards
        }

        Central.GlobalData.GameData.CurrentSpin = spinData;
        Central.GlobalData.GameData.LastOutCome = outcome;
        /*Central.GlobalData.GameData.SpinWin.Value = spinData.spinAward * Central.GlobalData.BetMultiplier;
        Central.GlobalData.GameData.TotalWon.Value = outcome.TotalAward * Central.GlobalData.BetMultiplier;*/

        if (demoSymbols == null)
        {
            GenerateReelsEndPosition(spinData);
        }
        else
        {
            Central.MathGenerator.Outcome = null;
            GenerateReelsEndPosition(demoSymbols);
        }

        Central.GlobalData.GameData.ReelsResult = GetReelsResult();

        Central.GlobalData.GameData.WinDetail = EvaluateWin();

        Central.GlobalData.GameData.TotalWon.Value = outcome.TotalAward * Central.GlobalData.BetMultiplier;
        Central.GlobalData.GameData.SpinWin.Value = Central.GlobalData.GameData.WinDetail.Pay * Central.GlobalData.BetMultiplier;


        if (Central.GlobalData.GameData.WinDetail == null && spinData.totalAward > 0)
        {
            Debugger.Instance.LogError("Couldn't find win detail... Syms: " + spinData.syms + "     Pay: " + spinData.spinAward);
        }
        else if(Central.GlobalData.GameData.WinDetail != null)
        {
            if(Central.GlobalData.GameData.SpinWin.Value != spinData.spinAward && demoSymbols == null)
            {
                Debugger.Instance.LogError("Wrong pay value " + Central.GlobalData.GameData.SpinWin.Value + " ... Syms: " + spinData.syms + "     Pay: " + spinData.spinAward);
            }
        }
    }

    private SymbolData GetSymbolDataByID(int symbol)
    {
        for (int s = 0; s < SymbolInfo.Length; s++)
        {
            if(SymbolInfo[s].SymbolID == symbol)
            {
                return SymbolInfo[s];
            }
        }

        Debugger.Instance.LogError("Couldn't find symbol data for symbol: " + symbol);
        return new SymbolData();
    }

    public WinDetail EvaluateWin()
    {
        WinDetail detail = new WinDetail();
        detail.PayMode = PayModes.None;

        SymbolData[] symbolData = SymbolInfo;

        for (int s = 0; s < symbolData.Length; s++)
        {
            int checkSymbol = symbolData[s].SymbolID;
            PayEntry[] payEntry = GetSymbolPay(checkSymbol);
            int count = 0;

            for (int i = 0; i < Central.GlobalData.GameData.ReelsResult.Length; i++)
            {
                int symbol = Central.GlobalData.GameData.ReelsResult[i][1];
                if (symbol == symbolData[s].SymbolID || (symbolData[s].DoesWildReplace && GetSymbolDataByID(symbol).Type == SymbolType.Wild))
                {
                    count++;
                }
            }

            if(symbolData[s].Type == SymbolType.MixedBar)
            {
                int b1 = 0;
                int b2 = 0;
                int b3 = 0;
                int w = 0;

                for (int i = 0; i < Central.GlobalData.GameData.ReelsResult.Length; i++)
                {
                    int symbol = Central.GlobalData.GameData.ReelsResult[i][1];
                    if (symbol == 2)
                    {
                        b1++;
                    }
                    else if (symbol == 3)
                    {
                        b2++;
                    }
                    else if (symbol == 4)
                    {
                        b3++;
                    }
                    else if (GetSymbolDataByID(symbol).Type == SymbolType.Wild)
                    {
                        w++;
                    }

                    if(b1+b2+b3+w == 3 && b1 != 3 && b2 != 3 && w != 3)
                    {
                        count = 3;
                    }
                }
            }

            for (int e = 0; e < payEntry.Length; e++)
            {
                if (payEntry[e].PayMode == PayModes.Line)
                {
                    for (int i = 0; i < payEntry[e].PayData.Length; i++)
                    {
                        if (count == payEntry[e].PayData[i].NumSymbols)
                        {
                            detail.SymbolID = checkSymbol;
                            detail.SymbolCount = count;
                            detail.PayMode = PayModes.Line;
                            detail.Pay = payEntry[e].PayData[i].Pay;
                        }
                    }
                }
                else if (payEntry[e].PayMode == PayModes.Scattered)
                {
                    for (int i = 0; i < payEntry[e].PayData.Length; i++)
                    {
                        if (count == payEntry[e].PayData[i].NumSymbols)
                        {
                            detail.SymbolID = checkSymbol;
                            detail.SymbolCount = count;
                            detail.PayMode = PayModes.Scattered;
                            detail.Pay = payEntry[e].PayData[i].Pay;
                        }
                    }
                }
            }
        }

        return detail;
    }

    public void SetDemoSymbols(int[] symbols)
    {
        _DemoSymbols = symbols;
    }

    public void GenerateReelsEndPosition(DazzleSpinData outcome)
    {
        List<int> stops = new List<int>();
        stops = Central.MathGenerator.StringToIntList(outcome.stops);

        if (stops.Count == ReelsController.ReelsEndPosition.Length)
        {
            for (int i = 0; i < ReelsController.ReelsEndPosition.Length; ++i)
            {
                ReelsController.ReelsEndPosition[i] = stops[i];
            }
        }
        else
        {
            Debug.LogError("Bz reel stops and slots generation stops have a length mismatch.");
        }
    }

    public void GenerateReelsEndPosition(int[] demoSymbols)
    {
        if (demoSymbols != null) // If we have a demo spin
        {
            if (demoSymbols.Length != ReelsController.ReelsEndPosition.Length)
            {
                Debugger.Instance.LogError("Incorrect amount of symbols provided: " + demoSymbols.Length);
            }
            else
            {
                for (int r = 0; r < ReelCount; r++)
                {
                    ReelsController.ReelsEndPosition[r] = GetEndPositionFromSymbol(r, demoSymbols[r]);
                }
            }
        }
    }

    private int[][] GetReelsResult()
    {
        int[][] result = new int[ReelCount][];

        for (int r = 0; r < ReelCount; r++)
        {
            result[r] = new int[3];

            for (int s = 0; s < result[r].Length; s++)
            {
                result[r][s] = ReelStrips[r].Symbols[Modulo(ReelsController.ReelsEndPosition[r] + s - ReelsController.ReelPaddingAmount, ReelStrips[r].Symbols.Length)];
            }
        }

        return result;
    }

    public int GetEndPositionFromSymbol(int r, int symbol)
    {
        List<int> rtn = new List<int>();

        for (int s = 0; s < ReelStrips[r].Symbols.Length; s++)
        {
            int check = ReelStrips[r].Symbols[s];

            if (check == symbol)
            {
                rtn.Add(s);
            }
        }

        if (rtn.Count == 0)
        {
            Debugger.Instance.LogError("Couldn't find symbol: " + symbol + " on reel strip " + r);
        }

        return rtn[Random.Range(0, rtn.Count)];
    }

    public int GetSymbolFromEndPos(int r, int endPos)
    {
        return ReelStrips[r].Symbols[endPos];
    }

    public Sprite GetSpriteByIndex(int reel, int index)
    {
        return SymbolInfo[ReelStrips[reel].Symbols[index]].Image;
    }

    public PayEntry[] GetSymbolPay(int symbol)
    {
        PayEntry[] rtn = new PayEntry[0];
        for (int i = 0; i < SymbolInfo.Length; i++)
        {
            if (SymbolInfo[i].SymbolID == symbol)
            {
                rtn = SymbolInfo[i].PayInfo;
            }
        }

        return rtn;
    }

    private string GetSymbolNameByID(int id)
    {
        return SymbolInfo[id].Name;
    }

    private string GetSymbolName(int reel, int index)
    {
        return SymbolInfo[ReelStrips[reel].Symbols[index]].Name;
    }

    private void LogReelsResult()
    {
        string str = "";
        int[][] reelsResult = Central.GlobalData.GameData.ReelsResult;
        for (int r = 0; r < reelsResult.Length; r++)
        {
            str += "Reel: " + r + ": ";
            for (int s = 0; s < reelsResult[r].Length; s++)
            {
                str += reelsResult[r][s] + " ";
            }
            str += "   ";
        }

        Debugger.Instance.Log("Reels Result: " + str);
    }

    private void LogPayline()
    {
        string str = "";
        int[][] reelsResult = Central.GlobalData.GameData.ReelsResult;

        for (int r = 0; r < reelsResult.Length; r++)
        {
            str += GetSymbolNameByID(reelsResult[r][1]) + " ";
        }

        Debugger.Instance.Log("Reels Result: " + str);
    }

    private void LogEndPos()
    {
        string str = "";
        int[][] reelsResult = Central.GlobalData.GameData.ReelsResult;

        for (int r = 0; r < reelsResult.Length; r++)
        {
            str += ReelsController.ReelsEndPosition[r] + " ";
        }

        Debugger.Instance.Log("Reels End Pos: " + str);
    }

    private int Modulo(int x, int m)
    {
        return (x % m + m) % m;
    }
}