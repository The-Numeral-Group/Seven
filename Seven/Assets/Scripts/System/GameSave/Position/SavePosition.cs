using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePosition : MonoBehaviour
{
    public List<GameObject> GameObjects = new List<GameObject>();
    public void saveObjectsPosition()
    {
        for (int i = 0; i < GameObjects.Count; i++)
        {
            GameObjects[i].GetComponent<ActorDataManager>().data.position.initialValue = GameObjects[i].transform.position;
        }
    }

}
