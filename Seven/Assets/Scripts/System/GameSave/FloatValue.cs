using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]

// This is a scriptable object that can be used to store initial and runtime float values.
// Doc: https://docs.google.com/document/d/1wAPtc6Zn7wKZmwxgdEOR4RIl68vwON8FJvhYxwQdzbo/edit
public class FloatValue : ScriptableObject
{
    public float initialValue;

    public float RuntimeValue;
}
