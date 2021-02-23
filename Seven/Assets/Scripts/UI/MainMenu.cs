﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Script for handling main menu
public class MainMenu : BaseUI
{
    //ptr to the button to be selected
    public Button startingButton;
    
    void Start()
    {
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