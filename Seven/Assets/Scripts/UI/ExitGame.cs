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

    public GameObject boss;
    public GameObject player;

    private ActorHealth bossHealth;
    private ActorHealth playerHealth;

    private MultiActor bossActor;

    public void Awake()
    {   
        playerHealth = player.GetComponent<ActorHealth>();

        bossActor = boss.GetComponent<MultiActor>();
        
        Time.timeScale = 1f;
        GAME_IS_PAUSED = false;
        pauseMenuUI.SetActive(false);
        gameOverUI.SetActive(false);

    }

    // This is for tutorial scene
    public void Update()
    {
        var currentActor = bossActor.transform.parent;
        bossHealth = currentActor.GetComponent<ActorHealth>();

        if (bossHealth.currentHealth == 0f || playerHealth.currentHealth == 0f)
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
        if (GAME_IS_PAUSED)
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
