using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickGameView : MonoBehaviour
{
    public Image BlackFilter;
    public GameObject PickReelsBackground;
    public GameObject PickBackground;
    public GameObject PickUI;
    public GameObject PickTiles;
    public GameObject BonusWinText;
    public ParticleSystem BonusWinParticle;

    private void Start()
    {
        TogglePickObjects(false);
    }

    public void TogglePickObjects(bool enabled)
    {
        PickReelsBackground.SetActive(enabled);
        PickBackground.SetActive(enabled);
        PickUI.SetActive(enabled);
        PickTiles.SetActive(enabled);
    }
}
