using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCapture : MonoBehaviour
{
    PhoneType i55 = new PhoneType("i55", 1);
    PhoneType i65 = new PhoneType("i65", 1);
    PhoneType i129 = new PhoneType("i129", 1);
    PhoneType CurrentPhone;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    int n = 0;
    string type = "55";

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrentPhone = i55;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrentPhone = i65;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CurrentPhone = i129;
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            string name = CurrentPhone.Type + "_" + CurrentPhone.Count + ".png";
            string folderPath = "Screenshots/";

            UnityEngine.ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, name));
            CurrentPhone.Count++;
        }
    }
}

public class PhoneType
{
    public string Type;
    public int Count;

    public PhoneType (string type, int count)
    {
        Type = type;
        Count = count;
    }
}
