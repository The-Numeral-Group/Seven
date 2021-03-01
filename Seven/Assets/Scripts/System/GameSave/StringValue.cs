using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]

// This is a scriptable object that can be used to store initial and runtime string values.
public class StringValue : ScriptableObject
{
    public string initialValue;

    public string RuntimeValue;
}
