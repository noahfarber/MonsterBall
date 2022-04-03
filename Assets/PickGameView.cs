using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickGameView : MonoBehaviour
{
    public Image BlackFilter;
    public GameObject PickReelsBackground;
    public GameObject PickBackground;
    public GameObject PickTiles;
    public GameObject BonusWinText;
    public ParticleSystem BonusWinParticle;

    public void Toggle(bool enabled)
    {
        PickReelsBackground.SetActive(enabled);
        PickBackground.SetActive(enabled);
        PickTiles.SetActive(enabled);
    }
}
