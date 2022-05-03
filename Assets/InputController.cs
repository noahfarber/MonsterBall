using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController Instance;
    public System.Action PlayButtonPressed;
    private List<InputBehaviour> InputList = new List<InputBehaviour>();

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        InputList.Add(new InputBehaviour((KeyCode.Space), InputBehaviorTypes.Spin));
        InputList.Add(new InputBehaviour((KeyCode.Escape), InputBehaviorTypes.Pause));
        InputList.Add(new InputBehaviour((KeyCode.C), InputBehaviorTypes.ChangeBet));
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < InputList.Count; i++)
        {
            if (Input.GetKeyDown(InputList[i].Key))
            {
                InputList[i].Pressed?.Invoke();
            }
        }
    }

    public void Register(KeyCode key, InputBehaviorTypes type)
    {

    }
}

public class InputBehaviour
{
    public KeyCode Key;
    public InputBehaviorTypes Type;
    public System.Action Pressed;

    public InputBehaviour(KeyCode key, InputBehaviorTypes type)
    {
        Key = key;
        Type = type;
    }
}

public enum InputBehaviorTypes
{
    Spin,
    ChangeBet,
    Pause,
}