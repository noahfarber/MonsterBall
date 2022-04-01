using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;    

public class ReelSpinController : MonoBehaviour
{
    public static ReelSpinController Instance;
    public Math Math;

    [HideInInspector] public bool Spinning = false;
    public int ReelCount = 3;
    public int VisibleSymbolsCount = 3;
    public int ReelPaddingAmount = 1;
    public int DisplayOffset = 1;

    public float DefaultSpinSpeed = 10f;
    public float[] DefaultSpinStopTimes = new float[3] { 1f, 1.5f, 2f };

    public int SymbolHeight = 360;
    public float ReelWidth = 5f;

    public GameObject ReelPrefab;
    public GameObject SymbolPrefab;
    
    public System.Action<int> ReelStopped;

    private int[] ReelPosition = new int[3] { 0, 0, 0 };
    public int[] ReelsEndPosition = new int[3] { 0, 5, 12 };
    private Reel[] Reels;

    private int[] _NumFinalSymbolsFilled = new int[3] { 0, 0, 0 };
    private ReelSymbol[] _PaylineSymbols = new ReelSymbol[3];

    private float[] _CurrentSpinSpeed = new float[3] { 10f, 10f, 10f };

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }    
    }

    private void Start()
    {
        GenerateReels();
        ReelStopped += OnReelStopped;
    }


    void Update()
    {
        CheckSpin();
    }

    public void GenerateReels()
    {
        Reels = new Reel[ReelCount];

        for (int r = 0; r < ReelCount; r++)
        {
            GameObject reelPrefab = Instantiate(ReelPrefab, transform);
            Reels[r] = reelPrefab.GetComponent<Reel>();
            Reels[r].SpinTimer = 0f;
            Reels[r].StopTime = DefaultSpinStopTimes[r];
            Reels[r].BounceAmount = .5f;

            reelPrefab.transform.position = new Vector3(-ReelWidth + (ReelWidth * r), 0f, 0f);

            for (int s = 0; s < VisibleSymbolsCount + (ReelPaddingAmount * 2); s++)
            {
                GameObject symbol = Instantiate(SymbolPrefab, reelPrefab.transform);
                ReelSymbol rSymbol = symbol.GetComponent<ReelSymbol>();
                Vector3 startPosition = new Vector3(reelPrefab.transform.position.x, ((SymbolHeight * ReelPaddingAmount * 2) - (SymbolHeight * s)) / 100f, 0f);
                int symbolPos = Modulo(s - ReelPaddingAmount - DisplayOffset, Math.ReelStrips[r].Symbols.Length);
                int symbolID = Math.ReelStrips[r].Symbols[symbolPos];
                symbol.transform.position = startPosition;

                rSymbol.Transform = symbol.transform;
                rSymbol.SpriteRenderer.sprite = Math.GetSpriteByIndex(r, symbolPos);
                Reels[r].Symbols.Add(rSymbol);
                Reels[r].LiveSymbols.Add(rSymbol);
                Reels[r].Symbols[s].Position = startPosition;
                Reels[r].Symbols[s].SymbolID = symbolID;
            }
        }
    }

    public void RequestStop()
    {
        if (AllReelsSpinning())
        {
            StopAllReels();
        }
    }
    
    public void Spin()
    {
        if(!Spinning)
        {
            Spinning = true;
            SoundConfig.Instance.PlayReelSpin();

            for (int r = 0; r < Reels.Length; r++)
            {
                _PaylineSymbols[r] = null;
                _NumFinalSymbolsFilled[r] = 0;
                _CurrentSpinSpeed[r] = DefaultSpinSpeed;
                Reels[r].State = ReelStates.Spinning;
                Reels[r].SpinTimer = 0f;
                Reels[r].StopTime = DefaultSpinStopTimes[r];
            }
        }
    }

    private void StopAllReels()
    {
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
                    if (_PaylineSymbols[r] != null && (_PaylineSymbols[r].Position.y + (_CurrentSpinSpeed[r] * Time.deltaTime * speedMult) >= reel.transform.position.y))
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

    private void MoveReelSymbols(int r, bool down, float speedMultiplier = 1f)
    {
        Reel reel = Reels[r];

        for (int s = 0; s < reel.Symbols.Count; s++)
        {
            reel.Symbols[s].Position.y += (_CurrentSpinSpeed[r] * Time.deltaTime * speedMultiplier) * (down ? -1f : 1f);

            if (reel.Symbols[s].Position.y <= -SymbolHeight * 3f / 100f)
            {
                ResetSymbolPosition(r, s);
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

    public ParticleSystem Anticipation;
    private void OnReelStopped(int r)
    {
        Reel reel = Reels[r];
        reel.State = ReelStates.Idle;
        SoundConfig.Instance.ReelStopped(r);

        if(Reels[0].State == ReelStates.Idle && r == 1) 
        {
            if (Central.GlobalData.GameData.ReelsResult[0][1] == 0 && Central.GlobalData.GameData.ReelsResult[1][1] == 0)
            {
                _CurrentSpinSpeed[2] *= 1.2f;
                Reels[2].StopTime += 3f;
                Anticipation.Play();
            }
        }

        for (int i = 0; i < Reels.Length; i++)
        {
            if(Reels[i].State != ReelStates.Idle)
            {
                return; // A reel is still spinning
            }
        }

        Anticipation.Stop();
        SoundConfig.Instance.StopReelSpin();
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
            int newReelPos = Modulo(-ReelsEndPosition[r] - Central.GlobalData.GameData.ReelsResult[r].Length + _NumFinalSymbolsFilled[r], Math.ReelStrips[r].Symbols.Length);
            int checkSymbolIndex = Modulo(-newReelPos - ReelPaddingAmount - DisplayOffset, Math.ReelStrips[r].Symbols.Length);

            ReelPosition[r] = newReelPos;
            _NumFinalSymbolsFilled[r]++;
            if (_NumFinalSymbolsFilled[r] == 2)  // We've filled the payline symbol
            {
                _PaylineSymbols[r] = symbol;
            }
        }

        int newSymbolIndex = Modulo(-ReelPosition[r] - ReelPaddingAmount - DisplayOffset, Math.ReelStrips[r].Symbols.Length);
        int symbolID = Math.ReelStrips[r].Symbols[newSymbolIndex];
        Reels[r].Symbols[s].SpriteRenderer.sprite = Math.GetSpriteByIndex(r, newSymbolIndex);
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
    public SymbolType Type;
    public int SymbolID;
    public Sprite Image;
    public Color AssociatedColor = Color.white;
    public PayEntry[] PayInfo;
    public bool DoesWildReplace = true;
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
    Scattered,
    None
}

[System.Serializable]
public enum SymbolType
{
    Normal,
    Wild,
    MixedBar,
    Blank,
    Bonus
}