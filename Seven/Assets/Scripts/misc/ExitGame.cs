using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Credit for code
//https://www.youtube.com/watch?v=JivuXdrIHK0
//Of course its the boy brackeys.
public class ExitGame : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject gameOverUI;
    public static bool GAME_IS_PAUSED = false;
    public static bool GAME_IS_OVER = false;

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

        Time.timeScale = 1f;
        GAME_IS_PAUSED = false;
        pauseMenuUI.SetActive(false);
        gameOverUI.SetActive(false);

    }

    // This is for tutorial scene
    public void Update()
    {
        if (boss.myHealth.currentHealth == 0f || player.myHealth.currentHealth == 1f)
        {
            GAME_IS_OVER = true;
            gameOverUI.SetActive(true);
            Time.timeScale = 0f;
            GAME_IS_PAUSED = true;
        }
    }

    // Exits the game
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
        else // Press Escape twice, exits the game.
        { 
            Exit();
        }
    }

    void OnResume()
    {
        if(GAME_IS_PAUSED)
        {
            Resume();
        }
        if (GAME_IS_OVER)
        {
            SceneManager.LoadScene("Tutorial");
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
        gameOverUI.SetActive(false);
        Time.timeScale = 1f;
        GAME_IS_PAUSED = false;
    }
    
}
