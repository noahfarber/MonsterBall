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

    public float ReelSpinSpeed = 10f;

    public int SymbolHeight = 360;
    public float ReelWidth = 5f;

    public GameObject ReelPrefab;
    public GameObject SymbolPrefab;

    public ReelStrip[] ReelStrips;
    public SymbolData[] SymbolInfo;

    private int[] ReelPosition = new int[3] { 0, 0, 0 };
    private int[] ReelsEndPosition = new int[3] { 0, 5, 12 };
    private int[][] ReelsResult;
    private Reel[] Reels;

    private int[] _NumFinalSymbolsFilled = new int[3] { 0, 0, 0 };
    private ReelSymbol[] _PaylineSymbols = new ReelSymbol[3];

    public System.Action<int> ReelStopped;

    private void Start()
    {
        GenerateReels();
        ReelStopped += OnReelStopped;
    }

    float[] _SpinStopTimes = new float[3] { 1.25f, 1.75f, 2.25f };

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(Spinning)
            {
                StopAllReels();
            }
            else
            {
                Spin();
            }
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
            Reels[r].BounceAmount = 2f;

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


    public void Spin()
    {
        Spinning = true;

        for (int r = 0; r < Reels.Length; r++)
        {
            _PaylineSymbols[r] = null;
            _NumFinalSymbolsFilled[r] = 0;
            Reels[r].State = ReelStates.Spinning;
            Reels[r].SpinTimer = 0f;
            ReelsEndPosition[r] = Random.Range(0, ReelStrips[r].Symbols.Length);

            if (ReelsResult[r] == null) { ReelsResult[r] = new int[3]; }
            
            for (int s = 0; s < ReelsResult[r].Length; s++)
            {
                ReelsResult[r][s] = ReelStrips[r].Symbols[Modulo(ReelsEndPosition[r] + s - ReelPaddingAmount, ReelStrips[r].Symbols.Length)];
            }

        }

        LogEndPos();
        LogReelsResult();
        LogPayline();
    }

    public void StopAllReels()
    {
        for (int r = 0; r < Reels.Length; r++)
        {
            Reels[r].State = ReelStates.Stopping;
            _NumFinalSymbolsFilled[r] = 0;
        }
    }

    public void StopReel(int r)
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
                if (reel.State == ReelStates.Stopping)
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
                if (reel.State == ReelStates.Bouncing)
                {
                    if (_PaylineSymbols[r] != null && (_PaylineSymbols[r].Position.y >= reel.transform.position.y))
                    {
                        ReelStopped?.Invoke(r);
                    }
                    else
                    {
                        MoveReelSymbols(r, false);
                    }
                }
            }
        }
    }

    private void MoveReelSymbols(int reelNumber, bool down)
    {
        Reel reel = Reels[reelNumber];

        for (int s = 0; s < reel.Symbols.Count; s++)
        {
            reel.Symbols[s].Position.y += (ReelSpinSpeed * Time.deltaTime) * (down ? -1f : 1f);

            if (reel.Symbols[s].Position.y <= -SymbolHeight * 3f / 100f)
            {
                ResetSymbolPosition(reelNumber, s);
            }


            reel.Symbols[s].Transform.position = reel.Symbols[s].Position;
        }
    }

    private void OnReelStopped(int r)
    {
        Reel reel = Reels[r];
        reel.State = ReelStates.Idle;

        for (int i = 0; i < Reels.Length; i++)
        {
            if(Reels[i].State == ReelStates.Spinning)
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

    private Sprite GetSpriteByIndex(int reel, int index)
    {
        return SymbolInfo[ReelStrips[reel].Symbols[index]].Image;
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
    public int Index;
    public Sprite Image;
}
