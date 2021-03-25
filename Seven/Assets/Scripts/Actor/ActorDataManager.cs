using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages all the stored actor data.
public class ActorDataManager : MonoBehaviour
{
    public ActorData data;

    public GameObject gameSaveManager;

    public void updateActorPosition(Vector2 newPosition)
    {
        data.position.RuntimeValue = newPosition;
        gameSaveManager.GetComponent<GameSaveManager>().SaveSaveObjects();
    }

    public void updateActorScene(string newScene)
    {
        data.currentScene.RuntimeValue = newScene;
        gameSaveManager.GetComponent<GameSaveManager>().SaveSaveObjects();
    }
}
