using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reel : MonoBehaviour
{
    public ReelStates State;
    public float BounceAmount;
    public List<ReelSymbol> Symbols = new List<ReelSymbol>();
    public List<ReelSymbol> LiveSymbols = new List<ReelSymbol>();
    public float SpinTimer = 0f;
    public float StopTime = 1f;
}

public enum ReelStates
{
    Idle,
    Spinning,
    Stopping,
    Bouncing
}