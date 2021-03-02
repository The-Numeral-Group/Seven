﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlacePosition : MonoBehaviour
{
    public List<GameObject> GameObjects = new List<GameObject>();
    //public List<VectorValue> ObjectPosition = new List<VectorValue>();

    public float newXPos { get; set; }
    public float newYPos { get; set; }

    // Place objects when scene loads in
    void Awake()
    {
        for (int i = 0; i < GameObjects.Count; i++)
        {
            GameObjects[i].transform.position = GameObjects[i].GetComponent<ActorDataManager>().data.position.initialValue;
        }
    }

    public void setObjectNewPosition(string name)
    {
        GameObject obj = GameObjects.Find((x) => x.name == name);
        if(obj == null)
        {
            Debug.LogWarning("Could not find " + name + "in GameObjects List");
        }
        obj.transform.position = new Vector2(newXPos, newYPos);
    }

}
