using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]

// This is a scriptable object that can be used to store initial and runtime bool values.
// Doc: https://docs.google.com/document/d/1epHiQ79tVp21QxfN-eJGV4MtPHVrnXQFbq1aPDF3zuw/edit
public class BoolValue : ScriptableObject
{
    public bool initialValue;

    public bool RuntimeValue;
}
