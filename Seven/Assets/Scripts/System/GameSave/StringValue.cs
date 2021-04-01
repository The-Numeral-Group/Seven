using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]

// This is a scriptable object that can be used to store initial and runtime string values.
// Doc: https://docs.google.com/document/d/1oU49NuS1hqV2YKm_8ivO3mdoAL-c_I9BIzqh66XDslE/edit
public class StringValue : ScriptableObject
{
    public string initialValue;

    public string RuntimeValue;
}
