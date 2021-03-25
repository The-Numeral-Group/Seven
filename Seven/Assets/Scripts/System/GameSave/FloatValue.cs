using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]

// This is a scriptable object that can be used to store initial and runtime float values.
public class FloatValue : ScriptableObject
{
    public float initialValue;

    public float RuntimeValue;
}
