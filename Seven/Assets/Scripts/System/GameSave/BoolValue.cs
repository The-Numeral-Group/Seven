using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]

// This is a scriptable object that can be used to store initial and runtime bool values.
public class BoolValue : ScriptableObject
{
    public bool initialValue;

    public bool RuntimeValue;
}
