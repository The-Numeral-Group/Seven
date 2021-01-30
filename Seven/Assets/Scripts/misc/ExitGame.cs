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

    private Actor boss;
    private Actor player;

    private ActorHealth bossHealth;
    private ActorHealth playerHealth;

    public void Awake()
    {
        var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];
        var bossObject = GameObject.FindGameObjectsWithTag("Boss")?[0];

        if (playerObject == null)
        {
            Debug.LogWarning("ExitGame: Cannot find player");
        }
        else
        {
            player = playerObject.GetComponent<Actor>();
        }

        if (bossObject == null)
        {
            Debug.LogWarning("ExitGame: Cannot find boss");
        }
        else
        {
            boss = bossObject.GetComponent<Actor>();
        }

    }

    // This is for tutorial scene
    public void Update()
    {
        if (boss.myHealth.currentHealth == 0f || player.myHealth.currentHealth <= 1f)
        {
            //EditorExit();
        }
    }

    // Exits the game
    public void Exit()
    {
        Application.Quit();
    }

    // Exits Unity Editor
    public void EditorExit()
    {
        UnityEditor.EditorApplication.isPlaying = false;
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
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GAME_IS_PAUSED = false;
    }
    
}
