using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : BaseUI
{
    //references to both buttons should be set via inspector
    [Tooltip("Reference to this ui objects restart button.")]
    public Button restartButton;
    [Tooltip("Reference to this ui objects exit button.")]
    public Button exitButton;

    public void CloseApplication()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public override void Show()
    {
        base.Show();
        restartButton.Select();
    }
}
