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
    IntProperty _TotalWon = new IntProperty();
    public IntProperty TotalWon
    {
        get { return _TotalWon; }
    }
}