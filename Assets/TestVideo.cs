using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TestVideo : MonoBehaviour
{
    public Transform[] Transforms;
    private VideoPlayer Player;
    private int clipIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        Player = GetComponent<VideoPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(InputBehaviorTypes.GetKeyDown(KeyCode.Tab))
        {
            for (int i = 0; i < Transforms.Length; i++)
            {
                if(Transforms[i] == Transforms[clipIndex % Transforms.Length])
                {
                    Transforms[i].localScale = Vector3.one;
                }
                else
                {
                    Transforms[i].localScale = Vector3.zero;
                }
            }

            clipIndex++;
        }
    }
}
