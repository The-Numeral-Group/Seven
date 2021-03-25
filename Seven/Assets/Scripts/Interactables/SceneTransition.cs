﻿using UnityEngine;
using UnityEngine.SceneManagement;

//Interactable object which can cause the scene to change.
public class SceneTransition : Interactable
{
    //Name of the scene that should be loaded when interacting with this object.
    [Tooltip("The name of the scene to be loaded.")]
    public string sceneToLoad = "";

    public override void OnInteract()
    {
        if (sceneToLoad == "")
        {
            Debug.LogWarning("SceneTranstion: No scene name provided for object " + this.gameObject.name);
            return;
        }
        SceneManager.LoadScene(sceneToLoad);
    }
}
