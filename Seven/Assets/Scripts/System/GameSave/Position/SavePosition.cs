using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Doc: https://docs.google.com/document/d/1D2_wIAB-If-33qhKYqsUUPh2Jfy8WRf7MuSL1LFJYYo/edit
public class SavePosition : MonoBehaviour
{
    public List<GameObject> GameObjects = new List<GameObject>();
    public void saveObjectsPosition()
    {
        for (int i = 0; i < GameObjects.Count; i++)
        {
            GameObjects[i].GetComponent<ActorDataManager>().data.position.RuntimeValue = GameObjects[i].transform.position;
        }
        this.gameObject.SendMessageUpwards("SaveSaveObjects");
    }

}
