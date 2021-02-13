using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Expected to be attached to the pause menu ui.
public class PauseMenu : BaseUI
{
    //Flag for if the game is paused
    public static bool GAME_IS_PAUSED;

    //Public Function which will pause the game
    public void PauseGame()
    {
        if (!GAME_IS_PAUSED)
        {
            Pause();
        }
        else
        {
            Hide();
        }
    }

    //Paused the game
    void Pause()
    {
        GAME_IS_PAUSED = true;
        Time.timeScale = 0f;
    }

    //Closes the game.
    public void CloseApplication()
    {
        Application.Quit();
    }

    //Overriding base class ui.
    public override void Hide()
    {
        GAME_IS_PAUSED = false;
        Time.timeScale = 1f;
        base.Hide();
    }
}
