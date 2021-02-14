using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : BaseUI
{
    public void CloseApplication()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
