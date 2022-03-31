using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class SoundConfig : MonoBehaviour
{
    public static SoundConfig Instance;

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
}
