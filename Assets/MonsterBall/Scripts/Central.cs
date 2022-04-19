using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Central
{
    static Math _math;
    public static Math Math
    {
        get
        {
            if (_math == null)
            {
                _math = Object.FindObjectOfType<Math>();
            }

            return _math;
        }
    }

    static GlobalData _globalData;
    public static GlobalData GlobalData
    {
        get
        {
            if (_globalData == null)
            {
                _globalData = Object.FindObjectOfType<GlobalData>();
            }

            return _globalData;
        }
    }
}