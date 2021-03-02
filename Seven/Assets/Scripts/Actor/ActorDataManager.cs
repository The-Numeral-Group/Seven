using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages all the stored actor data.
public class ActorDataManager : MonoBehaviour
{
    public ActorData data;

    public void updateActorPosition(Vector2 newPosition)
    {
        data.position.initialValue = newPosition;
    }

    public void updateActorScene(string newScene)
    {
        data.currentScene.initialValue = newScene;
    }
}
