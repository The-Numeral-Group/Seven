using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]

// This is a scriptable object that can be used to store initial and runtime vector values.
public class VectorValue : ScriptableObject
{
    public Vector2 initialValue;

    public Vector2 RuntimeValue;

}
