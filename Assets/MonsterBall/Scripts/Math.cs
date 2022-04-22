using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Framework;

public class Math : MonoBehaviour
{
    public static Math Instance;
    public ReelSpinController ReelsController;
    public ReelStrip[] ReelStrips;
    public SymbolData[] SymbolInfo;
    private int ReelCount = 3;
    private int[] _DemoSymbols;

    public int[] Outcome;
    private Dictionary<string, int[]> ReelWeights = new Dictionary<string, int[]>();
    private int[] ReelWeightCount = new int[3];

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
        Outcome = new int[3];
        LoadReels();
    }

    public int[] RequestOutcome()
    {
        string outcomeStr = "";
        for (int r = 0; r < Outcome.Length; r++)
        {
            int pointer = Random.Range(0, ReelWeightCount[r]);
            int weightCount = 0;
            foreach (var symbolWeights in ReelWeights)
            {
                if (pointer <= weightCount + symbolWeights.Value[r])
                {
                    Outcome[r] = GetEndPositionFromSymbolName(r, symbolWeights.Key);
                    outcomeStr += Outcome[r] + ", ";
                    break;

                }
                else
                {
                    weightCount += symbolWeights.Value[r];
                }
            }
        }

        if (Outcome != null)
        {
            Debugger.Instance.Log(outcomeStr);
        }
        else
        {
            Debugger.Instance.LogError("Couldn't get an outcome");
        }

        return Outcome;
    }
    public void GenerateOutcome(int[] demoSymbols = null)
    {
        RequestOutcome();

        if (demoSymbols == null)
        {
            GenerateReelsEndPosition(Outcome);
        }
        else
        {
            Outcome = null;

            GenerateReelsEndPosition(demoSymbols);
        }

        Central.GlobalData.GameData.ReelsResult = GetReelsResult();

        WinDetail winDetail = EvaluateWin();
        Central.GlobalData.GameData.TotalWon.Value = winDetail.Pay;
        Central.GlobalData.GameData.LastWinDetail = winDetail;

        LogPayline();
        LogEndPos();
        LogReelsResult();
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
            int[][] reelsResult = Central.GlobalData.GameData.ReelsResult;
            for (int i = 0; i < reelsResult.Length; i++)
            {
                int symbol = reelsResult[i][1];
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

                int b1SymbolID = 2;
                int b2SymbolID = 3;
                int b3SymbolID = 4;

                for (int i = 0; i < reelsResult.Length; i++)
                {
                    int check = reelsResult[i][1];
                    if (check >= 0)
                    {
                        if (GetSymbolDataByID(check).Type == SymbolType.Wild)
                        { w++; }

                        if (check == b1SymbolID)
                        { b1++; }
                        else if (check == b2SymbolID)
                        { b2++; }
                        else if (check == b3SymbolID)
                        { b3++; }
                    }
                }

                if (w + b1 + b2 + b3 == 3 && w < 3)
                {
                    if ((b1 > 0 && b2 > 0) ||
                        (b2 > 0 && b3 > 0) ||
                        (b1 > 0 && b3 > 0))
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

    public void GenerateReelsEndPosition(int[] stops)
    {
        if (stops.Length == ReelsController.ReelsEndPosition.Length)
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

    public SymbolData GetSymbolDataByID(int symbol)
    {
        for (int s = 0; s < SymbolInfo.Length; s++)
        {
            if (SymbolInfo[s].SymbolID == symbol)
            {
                return SymbolInfo[s];
            }
        }

        Debugger.Instance.LogError("Couldn't find symbol data for symbol: " + symbol);
        return new SymbolData();
    }

    public int GetEndPositionFromSymbolID(int r, int symbol)
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

    public int GetEndPositionFromSymbolName(int r, string symbolName)
    {
        List<int> rtn = new List<int>();

        for (int s = 0; s < SymbolInfo.Length; s++)
        {
            if(SymbolInfo[s].Name == symbolName)
            {

            }
        }
        for (int s = 0; s < ReelStrips[r].Symbols.Length; s++)
        {
            string checkName = GetSymbolDataByID(ReelStrips[r].Symbols[s]).Name;

            if (checkName == symbolName)
            {
                rtn.Add(s);
            }
        }

        if (rtn.Count == 0)
        {
            Debugger.Instance.LogError("Couldn't find symbol: " + symbolName + " on reel strip " + r);
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

    private string GetSymbolNameByEndPos(int reel, int endPos)
    {
        return SymbolInfo[ReelStrips[reel].Symbols[endPos]].Name;
    }

    private void LoadReels()
    {
        ReelWeights.Clear();
        ReelWeights.Add("Blank", new int[3] { 55, 55, 55 });
        ReelWeights.Add("Cherry", new int[3] { 65, 66, 70 });
        ReelWeights.Add("Bar1", new int[3] { 180, 190, 180 });
        ReelWeights.Add("Bar2", new int[3] { 150, 140, 135 });
        ReelWeights.Add("Bar3", new int[3] { 120, 110, 115 });
        ReelWeights.Add("Bell", new int[3] { 100, 90, 100 });
        ReelWeights.Add("Seven", new int[3] { 95, 105, 120 });
        ReelWeights.Add("Diamond", new int[3] { 65, 75, 85 });
        ReelWeights.Add("Wild", new int[3] { 60, 60, 60 });
        ReelWeights.Add("Bonus", new int[3] { 110, 109, 80 });

        string weights = "";
        for (int r = 0; r < ReelWeightCount.Length; r++)
        {
            for (int i = 0; i < ReelWeights.Count; i++)
            {
                ReelWeightCount[r] += ReelWeights.ElementAt(i).Value[0];
            }

            weights += "Reel " + r + " Weight: " + ReelWeightCount[r] + "  ";
        }

        Debugger.Instance.Log(weights);
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

    private List<int> StringToIntList(string str)
    {
        List<int> rtn = new List<int>();

        if (!string.IsNullOrEmpty(str))
        {
            foreach (var s in str.Split(','))
            {
                int num;
                if (int.TryParse(s, out num))
                {
                    rtn.Add(num);
                }
                else
                {
                    Debug.LogError("Couldn't convert string : " + s);
                }
            }
        }

        return rtn;
    }
}
