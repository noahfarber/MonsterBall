using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveOnStart : MonoBehaviour
{
    [SerializeField] private bool Enabled;

    void Start()
    {
        gameObject.SetActive(Enabled);
    }
}
