using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Document Link: https://docs.google.com/document/d/1s8NkI1jXcX2C8ExWdVcrrcUZx5JqrdN3u87krEySkFo/edit?usp=sharing
//Handles the game over ui elements.
public class GameOver : BaseUI
{
    //references to both buttons should be set via inspector
    [Tooltip("Reference to this ui objects restart button.")]
    public Button restartButton;
    [Tooltip("Reference to this ui objects exit button.")]
    public Button exitButton;

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
