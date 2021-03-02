using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveScene : MonoBehaviour
{
    public List<GameObject> GameObjects = new List<GameObject>();
    public void SaveCurrentScene()
    {
        for (int i = 0; i < GameObjects.Count; i++)
        {
            GameObjects[i].GetComponent<ActorDataManager>().data.currentScene.RuntimeValue = SceneManager.GetActiveScene().name;
        }
    }

}
