using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveScene : MonoBehaviour
{
    public List<GameObject> GameObjects = new List<GameObject>();
    public void Awake()
    {
        for (int i = 0; i < GameObjects.Count; i++)
        {
            Debug.Log(SceneManager.GetActiveScene().name);
            GameObjects[i].GetComponent<ActorDataManager>().data.currentScene.initialValue = SceneManager.GetActiveScene().name;
            Debug.Log(GameObjects[i].GetComponent<ActorDataManager>().data.currentScene.initialValue);
        }
    }

}
