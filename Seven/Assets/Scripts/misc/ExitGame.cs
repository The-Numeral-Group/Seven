using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Credit for code
//https://www.youtube.com/watch?v=JivuXdrIHK0
//Of course its the boy brackeys.
public class ExitGame : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public static bool GAME_IS_PAUSED = false;
    public void Exit()
    {
        Application.Quit();
    }
    
    void OnMenu()
    {
        if (!GAME_IS_PAUSED)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }

    void Pause()
    {
        GAME_IS_PAUSED = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    void Resume()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 1f;
        GAME_IS_PAUSED = false;
    }
    
}
