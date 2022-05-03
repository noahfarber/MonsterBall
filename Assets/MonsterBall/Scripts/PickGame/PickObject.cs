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
    public ParticleSystem GoodLoop;
    public ParticleSystem BadLoop;


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
        GoodLoop.Stop();
        BadLoop.Stop();
    }

    public void Clicked(int value)
    {
        bool isBomb = value == -1;
        ValueText.text = isBomb ? "X" : value.ToString();
        Open = true;
        GetComponent<Button>().enabled = false;
        PressedParticle.Play();
        if(isBomb)
        {
            GoodLoop.Play();
        }
        else
        {
            BadLoop.Play();
        }
    }

}
