using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;    

public class ReelSpinController : MonoBehaviour
{
    [HideInInspector] public bool Spinning = false;
    public int ReelCount = 3;
    public int VisibleSymbolsCount = 3;
    public int ReelPaddingAmount = 1;
    public int DisplayOffset = 1;

    public float DefaultSpinSpeed = 10f;

    public int SymbolHeight = 360;
    public float ReelWidth = 5f;

    public GameObject ReelPrefab;
    public GameObject SymbolPrefab;

    public ReelStrip[] ReelStrips;
    public SymbolData[] SymbolInfo;
    
    public System.Action<int> ReelStopped;

    private int[] ReelPosition = new int[3] { 0, 0, 0 };
    private int[] ReelsEndPosition = new int[3] { 0, 5, 12 };
    private int[][] ReelsResult;
    private Reel[] Reels;

    private int[] _NumFinalSymbolsFilled = new int[3] { 0, 0, 0 };
    private ReelSymbol[] _PaylineSymbols = new ReelSymbol[3];

    private float _CurrentSpinSpeed;

    private void Start()
    {
        GenerateReels();
        ReelStopped += OnReelStopped;
    }

    float[] _SpinStopTimes = new float[3] { 1.25f, 1.75f, 2.25f };

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RequestSpin();
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            RequestSpin(new int[3] { 0, 0, 0 });
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            RequestSpin(new int[3] { 1, 1, 1 });
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            RequestSpin(new int[3] { 2, 2, 2 });
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            RequestSpin(new int[3] { 3, 3, 3 });
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            RequestSpin(new int[3] { 4, 4, 4 });
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            RequestSpin(new int[3] { 5, 5, 5 });
        }

        CheckSpin();
    }

    public void GenerateReels()
    {
        Reels = new Reel[ReelCount];
        ReelsResult = new int[ReelCount][];

        for (int r = 0; r < ReelCount; r++)
        {
            GameObject reelPrefab = Instantiate(ReelPrefab, transform);
            Reels[r] = reelPrefab.GetComponent<Reel>();
            Reels[r].SpinTimer = 0f;
            Reels[r].StopTime = _SpinStopTimes[r];
            Reels[r].BounceAmount = .5f;

            reelPrefab.transform.position = new Vector3(-ReelWidth + (ReelWidth * r), 0f, 0f);

            for (int s = 0; s < VisibleSymbolsCount + (ReelPaddingAmount * 2); s++)
            {
                GameObject symbol = Instantiate(SymbolPrefab, reelPrefab.transform);
                ReelSymbol rSymbol = symbol.GetComponent<ReelSymbol>();
                Vector3 startPosition = new Vector3(reelPrefab.transform.position.x, ((SymbolHeight * ReelPaddingAmount * 2) - (SymbolHeight * s)) / 100f, 0f);
                int symbolPos = Modulo(s - ReelPaddingAmount - DisplayOffset, ReelStrips[r].Symbols.Length);
                int symbolID = ReelStrips[r].Symbols[symbolPos];
                symbol.transform.position = startPosition;

                rSymbol.Transform = symbol.transform;
                rSymbol.SpriteRenderer.sprite = GetSpriteByIndex(r, symbolPos);
                Reels[r].Symbols.Add(rSymbol);
                Reels[r].LiveSymbols.Add(rSymbol);
                Reels[r].Symbols[s].Position = startPosition;
                Reels[r].Symbols[s].SymbolID = symbolID;
            }
        }
    }

    public void RequestSpin(int[] symbolIDs = null)
    {
        if (!Spinning)
        {
            Spin(symbolIDs);
        }
        else if (AllReelsSpinning())
        {
            StopAllReels();
        }
    }

    private void Spin(int[] symbolIDs = null)
    {
        Spinning = true;
        _CurrentSpinSpeed = DefaultSpinSpeed;

        for (int r = 0; r < Reels.Length; r++)
        {
            _PaylineSymbols[r] = null;
            _NumFinalSymbolsFilled[r] = 0;
            Reels[r].State = ReelStates.Spinning;
            Reels[r].SpinTimer = 0f;

            if (symbolIDs == null)
            {
                ReelsEndPosition[r] = Random.Range(0, ReelStrips[r].Symbols.Length);
            }
            else if (ReelsEndPosition.Length != symbolIDs.Length)
            {
                Debugger.Instance.LogError("Incorrect amount of symbols provided: " + symbolIDs.Length);
            }
            else
            {
                ReelsEndPosition[r] = GetEndPositionFromSymbol(r, symbolIDs[r]);
            }

            if (ReelsResult[r] == null) { ReelsResult[r] = new int[3]; }
            
            for (int s = 0; s < ReelsResult[r].Length; s++)
            {
                ReelsResult[r][s] = ReelStrips[r].Symbols[Modulo(ReelsEndPosition[r] + s - ReelPaddingAmount, ReelStrips[r].Symbols.Length)];
            }

        }

        //LogEndPos();
        //LogReelsResult();
        LogPayline();
    }

    public List<WinDetail> EvaluateWin()
    {
        List<WinDetail> details = new List<WinDetail>();

        for (int s = 0; s < SymbolInfo.Length; s++)
        {
            int symbolID = SymbolInfo[s].SymbolID;
            PayEntry[] payEntry = GetSymbolPay(symbolID);
            int count = 0;

            for (int i = 0; i < ReelsEndPosition.Length; i++)
            {
                int symbol = GetSymbolFromEndPos(i, ReelsEndPosition[i]);
                if(symbol == SymbolInfo[s].SymbolID)
                {
                    count++;
                }
            }

            for (int e = 0; e < payEntry.Length; e++)
            {
                if(payEntry[e].PayMode == PayModes.Line)
                {
                    for (int i = 0; i < payEntry[e].PayData.Length; i++)
                    {
                        if (count == payEntry[e].PayData[i].NumSymbols)
                        {
                            WinDetail detailWon = new WinDetail();
                            detailWon.SymbolID = symbolID;
                            detailWon.SymbolCount = count;
                            detailWon.Pay = payEntry[e].PayData[i].Pay;
                            details.Add(detailWon);
                        }
                    }
                }
                //else if( )
            }
        }

        return details;
    }

    private void StopAllReels()
    {
        //_CurrentSpinSpeed = DefaultSpinSpeed * 1.5f;
        for (int r = 0; r < Reels.Length; r++)
        {
            Reels[r].State = ReelStates.Stopping;
            _NumFinalSymbolsFilled[r] = 0;
        }
    }

    private void StopReel(int r)
    {
        Reels[r].State = ReelStates.Stopping;
    }

    private void CheckSpin()
    {
        if (Spinning)
        {
            for (int r = 0; r < Reels.Length; r++)
            {
                Reel reel = Reels[r];

                if (reel.State == ReelStates.Spinning)
                {
                    MoveReelSymbols(r, true);

                    if (reel.SpinTimer > reel.StopTime)
                    {
                        reel.State = ReelStates.Stopping;
                    }
                    else
                    {
                        reel.SpinTimer += Time.deltaTime;
                    }
                }
                else if (reel.State == ReelStates.Stopping)
                {
                    if (_PaylineSymbols[r] != null && (_PaylineSymbols[r].Position.y <= reel.transform.position.y - Reels[r].BounceAmount))
                    {
                        Reels[r].State = ReelStates.Bouncing;
                    }
                    else
                    {
                        MoveReelSymbols(r, true);
                    }
                }
                else if (reel.State == ReelStates.Bouncing)
                {
                    float speedMult = .75f;
                    if (_PaylineSymbols[r] != null && (_PaylineSymbols[r].Position.y + (_CurrentSpinSpeed * Time.deltaTime * speedMult) >= reel.transform.position.y))
                    {
                        SnapReelSymbols(r);
                        //MoveReelSymbols(r, false, true);
                        ReelStopped?.Invoke(r);
                    }
                    else
                    {
                        MoveReelSymbols(r, false, speedMult);
                    }
                }
            }
        }
    }

    private void MoveReelSymbols(int reelNumber, bool down, float speedMultiplier = 1f)
    {
        Reel reel = Reels[reelNumber];

        for (int s = 0; s < reel.Symbols.Count; s++)
        {
            reel.Symbols[s].Position.y += (_CurrentSpinSpeed * Time.deltaTime * speedMultiplier) * (down ? -1f : 1f);

            if (reel.Symbols[s].Position.y <= -SymbolHeight * 3f / 100f)
            {
                ResetSymbolPosition(reelNumber, s);
            }

            reel.Symbols[s].Transform.position = reel.Symbols[s].Position;
        }
    }

    private void SnapReelSymbols(int reelNumber)
    {
        Reel reel = Reels[reelNumber];

        for (int i = 0; i < reel.LiveSymbols.Count; i++)
        {
            ReelSymbol symbol = reel.LiveSymbols[i];
            Vector3 position = new Vector3(symbol.Position.x, ((SymbolHeight * ReelPaddingAmount * 2) - (SymbolHeight * Modulo(i, reel.Symbols.Count))) / 100f, 0f);
            symbol.Position = position;
            symbol.Transform.position = symbol.Position;
        }
    }

    private void OnReelStopped(int r)
    {
        Reel reel = Reels[r];
        reel.State = ReelStates.Idle;

        for (int i = 0; i < Reels.Length; i++)
        {
            if(Reels[i].State != ReelStates.Idle)
            {
                return; // A reel is still spinning
            }
        }

        Spinning = false;
    }

    private void ResetSymbolPosition(int r, int s)
    {
        ReelSymbol symbol = Reels[r].Symbols[s];
        Reels[r].LiveSymbols.Remove(symbol);

        List<ReelSymbol> otherSymbols = new List<ReelSymbol>(Reels[r].LiveSymbols);
        symbol.Position.y += SymbolHeight * 5f / 100f;
        
        Reels[r].LiveSymbols.Clear();
        Reels[r].LiveSymbols.Add(symbol);
        Reels[r].LiveSymbols.AddRange(otherSymbols);
        ReelPosition[r]++;

        if (Reels[r].State == ReelStates.Stopping)
        {
            int newReelPos = Modulo(-ReelsEndPosition[r] - ReelsResult[r].Length + _NumFinalSymbolsFilled[r], ReelStrips[r].Symbols.Length);
            int checkSymbolIndex = Modulo(-newReelPos - ReelPaddingAmount - DisplayOffset, ReelStrips[r].Symbols.Length);

            /*if((ReelStrips[r].Symbols[checkSymbolIndex] == 0 && symbol.SymbolID == 0) ||
                (ReelStrips[r].Symbols[checkSymbolIndex] != 0 && symbol.SymbolID != 0))
            {
                
            }*/

            ReelPosition[r] = newReelPos;
            _NumFinalSymbolsFilled[r]++;
            if (_NumFinalSymbolsFilled[r] == 2)  // We've filled the payline symbol
            {
                _PaylineSymbols[r] = symbol;
            }
        }

        int newSymbolIndex = Modulo(-ReelPosition[r] - ReelPaddingAmount - DisplayOffset, ReelStrips[r].Symbols.Length);
        int symbolID = ReelStrips[r].Symbols[newSymbolIndex];
        Reels[r].Symbols[s].SpriteRenderer.sprite = GetSpriteByIndex(r, newSymbolIndex);
        Reels[r].Symbols[s].SymbolID = symbolID;
    }

    private bool AllReelsSpinning()
    {
        bool rtn = true;
        for (int i = 0; i < Reels.Length; i++)
        {
            if(Reels[i].State != ReelStates.Spinning)
            {
                rtn = false;
            }
        }

        return rtn;
    }

    private int GetEndPositionFromSymbol(int r, int symbol)
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

    private int GetSymbolFromEndPos(int r, int endPos)
    {
        return ReelStrips[r].Symbols[endPos];
    }

    private Sprite GetSpriteByIndex(int reel, int index)
    {
        return SymbolInfo[ReelStrips[reel].Symbols[index]].Image;
    }

    private PayEntry[] GetSymbolPay(int symbol)
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

        for (int r = 0; r < ReelsResult.Length; r++)
        {
            str += "Reel: " + r + ": ";
            for (int s = 0; s < ReelsResult[r].Length; s++)
            {
                str += ReelsResult[r][s] + " ";
            }
            str += "   ";
        }

        Debugger.Instance.Log("Reels Result: " + str);
    }

    private void LogPayline()
    {
        string str = "";

        for (int r = 0; r < ReelsResult.Length; r++)
        {
            str += GetSymbolNameByID(ReelsResult[r][1]) + " ";
        }

        Debugger.Instance.Log("Reels Result: " + str);
    }

    private void LogEndPos()
    {
        string str = "";

        for (int r = 0; r < ReelsResult.Length; r++)
        {
            str += ReelsEndPosition[r] + " ";
        }

        Debugger.Instance.Log("Reels End Pos: " + str);
    }

    private int Modulo(int x, int m)
    {
        return (x % m + m) % m;
    }
}

[System.Serializable]
public class ReelStrip
{
    public int[] Symbols;
}

[System.Serializable]
public class SymbolData
{
    public string Name;
    public int SymbolID;
    public Sprite Image;
    public PayEntry[] PayInfo;
}

[System.Serializable]
public struct PayEntry
{
    public PayModes PayMode;
    public PayData[] PayData;
}

[System.Serializable]
public struct PayData
{
    public int NumSymbols;
    public int Pay;
}

[System.Serializable]
public enum PayModes
{
    Line,
    Scattered
}

public class WinDetail
{
    public int SymbolID;
    public int SymbolCount;
    public int Pay;
}