using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PickObject : MonoBehaviour
{
    public bool Open = false;
    public TextMeshProUGUI ValueText;
    public ParticleSystem PressedParticle;
    public ParticleSystem OpenLoopParticle;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clear()
    {
        ValueText.text = "";
        Open = false;
        GetComponent<Button>().enabled = true;
        OpenLoopParticle.Stop();
    }

    public void Clicked(int value)
    {
        ValueText.text = value == -1 ? "X" : value.ToString();
        ValueText.color = value == -1 ? Color.red : new Color(255, 0, 213);
        Open = true;
        GetComponent<Button>().enabled = false;
        PressedParticle.Play();
        OpenLoopParticle.Play();
        PressedParticle.startColor = value == -1 ? Color.red : Color.white;
        OpenLoopParticle.startColor = value == -1 ? Color.red : new Color(255, 0, 213);
    }

}
