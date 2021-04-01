using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]

// This is a scriptable object that can be used to store initial and runtime vector values.
// Doc: https://docs.google.com/document/d/1SoYX9HdcVF5L6EU2LZKYOh-1yFnp-4O6d73-dnl08BY/edit
public class VectorValue : ScriptableObject
{
    public Vector2 initialValue;

    public Vector2 RuntimeValue;

}
