using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Central
{
    static bzMathGenerator _mathGenerator;
    public static bzMathGenerator MathGenerator
    {
        get
        {
            if (_mathGenerator == null)
            {
                _mathGenerator = Object.FindObjectOfType<bzMathGenerator>();
            }

            return _mathGenerator;
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