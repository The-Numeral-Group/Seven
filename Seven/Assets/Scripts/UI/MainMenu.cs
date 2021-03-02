using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Script for handling main menu
public class MainMenu : BaseUI
{
    //ptr to the button to be selected
    public Button startingButton;
    //public GameObject gameSaveManager;

    public GameObject playerObject;
    
    void Start()
    {
        if (startingButton)
        {
            startingButton.Select();
        }
        /*if (gameSaveManager == null)
        {
            Debug.LogWarning("Cannot find gameSaveManager object!");
        }
        else
        {
            gameSaveManager.GetComponent<GameSaveManager>().ResetScriptables();
        }*/
    }
    //Activates the submenu, as well as selects tha passed in button argument.
    public void ShowSubMenu(GameObject menu)
    {
        menu.SetActive(true);
        if (startingButton)
        {
            startingButton.Select();
        }
        //button.Select();
    }

    public void SetStartingButton(Button ptr)
    {
        startingButton = ptr;
    }

    public void loadPlayerSavedScene()
    {
        SceneManager.LoadScene(playerObject.GetComponent<ActorDataManager>().data.currentScene.RuntimeValue);
    }
}
