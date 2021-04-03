using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Doc: https://docs.google.com/document/d/1ENgyvbT6TmqiSnaM3qbuXytnLzon49c_RYg4QEDJ5E8/edit
public class SaveScene : MonoBehaviour
{
    public List<GameObject> GameObjects = new List<GameObject>();
    public BoolValue newGame;
    public void SaveCurrentScene()
    {
        newGame.initialValue = false;
        for (int i = 0; i < GameObjects.Count; i++)
        {
            GameObjects[i].GetComponent<ActorDataManager>().data.currentScene.RuntimeValue = SceneManager.GetActiveScene().name;
        }
        this.gameObject.SendMessageUpwards("SaveSaveObjects");
    }

}
