using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class SoundConfig : MonoBehaviour
{
    public static SoundConfig Instance;

    public AudioSource ReelSpin;
    public AudioSource[] ReelStopSources;
    public AudioClip[] SpecialStops;
    public AudioClip ReelStop;

    public AudioSource BackgroundMusic;
    public AudioClip[] BackgroundClips;
    private int ClipIndex = 0;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if(!BackgroundMusic.isPlaying)
        {
            PlayNextBackgroundClip();
        }
    }

    public void PlayRandomBackgroundClip()
    {
        ClipIndex = Random.Range(0, BackgroundClips.Length);

        BackgroundMusic.clip = BackgroundClips[ClipIndex];
        SoundManager.Instance.PlayAndFade(BackgroundMusic, 1f, 1f, 0f);
    }

    public void PlayNextBackgroundClip()
    {
        ClipIndex++;

        if(ClipIndex >= BackgroundClips.Length)
        {
            ClipIndex = 0;
        }

        BackgroundMusic.clip = BackgroundClips[ClipIndex];
        SoundManager.Instance.PlayAndFade(BackgroundMusic, 1f, 1f, 0f);
    }

    public void PlayReelSpin()
    {
        SoundManager.Instance.Play(ReelSpin, 1f);
    }
    
    public void StopReelSpin()
    {
        SoundManager.Instance.Stop(ReelSpin);
    }

    public void ReelStopped(int r)
    {
        AudioClip stopSound = ReelStop;

        if(Central.GlobalData.GameData.ReelsResult[r][1] == 10) // If it's a bonus symbol
        {
            stopSound = SpecialStops[r];
        }

        SoundManager.Instance.PlayOneShot(stopSound, 1f);
    }
}
