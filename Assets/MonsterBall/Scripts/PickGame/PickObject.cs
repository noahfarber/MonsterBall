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
    public ParticleSystem OpenParticle;
    

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
    }

    public void Clicked(int value)
    {
        ValueText.text = value == -1 ? "X" : value.ToString();
        OpenParticle.Play();
        Open = true;
        GetComponent<Button>().enabled = false;
    }

}
