using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubDoorTransition : Interactable
{
    [Tooltip("The ID of the bossOpening ScriptableObject.")]
    public int bossID;

    //Name of the scene that should be loaded when interacting with this object.
    [Tooltip("The name of the scene to be loaded.")]
    public string sceneToLoad = "";

    public GameObject gameSaveManager;

    private GameSaveManager gameSaveManagerScript;

    private void Start()
    {
        this.gameSaveManagerScript = gameSaveManager.GetComponent<GameSaveManager>();
    }


    public override void OnInteract()
    {
        if(bossID == 0)
        {
            Debug.LogWarning("SceneTranstion: No bossID provided for object " + this.gameObject.name);
            return;
        }
        if (sceneToLoad == "")
        {
            Debug.LogWarning("SceneTranstion: No scene name provided for object " + this.gameObject.name);
            return;
        }

        // This will be used once opening cutscene has been implemented.
        /*if (this.gameSaveManagerScript.getBoolValue(bossID))
        {
            // Load Opening Cutscene
        }
        else
        {
            
        }*/

        SceneManager.LoadScene(sceneToLoad);
    }
}
