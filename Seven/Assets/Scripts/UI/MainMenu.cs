using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Script for handling main menu
public class MainMenu : BaseUI
{
    //ptr to the button to be selected
    public Button startingButton;

    protected override void Awake()
    {
        base.Awake();
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Start()
    {
        // Putting timeScale back to 1. (For returning back to the main menu)
        // This was needed for ButtonTextChange's changeTextDuration function
        Time.timeScale = 1;

        if (startingButton)
        {
            startingButton.Select();
        }
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

}
