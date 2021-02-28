using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePosition : MonoBehaviour
{
    public List<GameObject> GameObjects = new List<GameObject>();
    public List<VectorValue> ObjectPosition = new List<VectorValue>();
    public void saveObjectsPosition()
    {
        for (int i = 0; i < GameObjects.Count; i++)
        {
            ObjectPosition[i].initialValue = GameObjects[i].transform.position;
        }
    }
}
