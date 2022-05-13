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

    public void PlayNextBackgroundClip()
    {
        ClipIndex++;

        if(ClipIndex >= BackgroundClips.Length)
        {
            ClipIndex = 0;
        }

        BackgroundMusic.clip = BackgroundClips[ClipIndex];
        SoundManager.Instance.PlayAndFade(BackgroundMusic, .75f, 2f, 0f);
    }

    public void FadeBackgroundMusic(float targetVolume, float duration, float startVolume = -1)
    { 
        SoundManager.Instance.PlayAndFade(BackgroundMusic, targetVolume, duration, startVolume);
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
        bool playSound = true;
        for (int i = 0; i < 3; i++)
        {
            if (Central.GlobalData.GameData.ReelsResult[i][1] != 10 || Central.GlobalData.GameData.ReelsResult[r][1] != 10) // If it's a bonus symbol
            {
                playSound = false;
            }

            if(i >= r)
            {
                break;
            }
        }

        if(playSound)
        {
            stopSound = SpecialStops[r];
        }

        SoundManager.Instance.PlayOneShot(stopSound, 1f);
    }
}
