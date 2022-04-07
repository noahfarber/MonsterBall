using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinDurationConfig : MonoBehaviour
{
    public WinSoundDetail[] SoundConfig;
}

[System.Serializable]
public class WinSoundDetail
{
    public float MaxBetMultiple;
    public float Duration;
    public AudioClip Clip;
}