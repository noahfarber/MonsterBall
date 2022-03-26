using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseProperty<ValueType>
{
    bool _allowPropertyChange = true;
    [Newtonsoft.Json.JsonIgnore]
    public bool AllowPropertyChanged 
    {
        get { return _allowPropertyChange; }
        set
        {
            _allowPropertyChange = value;
            ExecuteProperty ();
        }
    }
    [SerializeField]
    ValueType _value;
    public ValueType Value
    {
        get { return _value; }
        set
        {
            _value = value;
            ExecuteProperty ();
        }
    }

    public System.Action OnPropertyChanged { get; set; }

    public static implicit operator ValueType( BaseProperty<ValueType> property )
    {
        return property.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    void ExecuteProperty()
    {
        if(_allowPropertyChange)
        {
            OnPropertyChanged?.Invoke();
        }
    }
}
