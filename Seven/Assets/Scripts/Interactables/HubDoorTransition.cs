using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubDoorTransition : MonoBehaviour
{
    [Tooltip("The ID of the bossOpening ScriptableObject.")]
    public int bossID;
    public int bossDefeatedIndex;
    public string postFightScene;

    public string openingScene;

    public string retryScene;

    public GameObject gameSaveManager;

    private GameSaveManager gameSaveManagerScript;

    private void Start()
    {
        this.gameSaveManagerScript = gameSaveManager.GetComponent<GameSaveManager>();
        checkParticleEffects();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (bossID == 0)
        {
            Debug.LogWarning("SceneTranstion: No bossID provided for object " + this.gameObject.name);
            return;
        }
        if (openingScene == "")
        {
            Debug.LogWarning("SceneTranstion: No opening scene name provided for object " + this.gameObject.name);
            return;
        }
        if (retryScene == "")
        {
            Debug.LogWarning("SceneTranstion: No retry scene name provided for object " + this.gameObject.name);
            return;
        }
        if (!this.gameSaveManagerScript.getBoolValue(bossDefeatedIndex))
        {
            // This will be used once opening cutscene has been implemented.
            if (this.gameSaveManagerScript.getBoolValue(bossID))
            {
                GameSettings.SCENE_TO_LOAD = openingScene;
            }
            else
            {
                GameSettings.SCENE_TO_LOAD = retryScene;
            }
        }
        else
        {
            GameSettings.SCENE_TO_LOAD = postFightScene;
        }
        SceneManager.LoadScene("LoadScreen");
    }

    private void checkParticleEffects()
    {

    }


    /*public override void OnInteract()
    {
        if(bossID == 0)
        {
            Debug.LogWarning("SceneTranstion: No bossID provided for object " + this.gameObject.name);
            return;
        }
        if (openingScene == "")
        {
            Debug.LogWarning("SceneTranstion: No opening scene name provided for object " + this.gameObject.name);
            return;
        }
        if (retryScene == "")
        {
            Debug.LogWarning("SceneTranstion: No retry scene name provided for object " + this.gameObject.name);
            return;
        }

        if (!this.gameSaveManagerScript.getBoolValue(bossDefeatedIndex))
        {
            // This will be used once opening cutscene has been implemented.
            if (this.gameSaveManagerScript.getBoolValue(bossID))
            {
                GameSettings.SCENE_TO_LOAD = openingScene;
            }
            else
            {
                GameSettings.SCENE_TO_LOAD = retryScene;
            }
        }
        else
        {
            GameSettings.SCENE_TO_LOAD = postFightScene;
        }
        SceneManager.LoadScene("LoadScreen");
    }*/
}
