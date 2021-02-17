using UnityEngine;
using UnityEngine.UI;

//Expected to be attached to the pause menu ui.
public class PauseMenu : BaseUI
{
    //Flag for if the game is paused
    public static bool GAME_IS_PAUSED;
    //exitButton should be set through inspector;
    public Button exitButton;

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
    
    //Overriding base class ui.
    public override void Hide()
    {
        GAME_IS_PAUSED = false;
        Time.timeScale = 1f;
        base.Hide();
    }

    public override void Show()
    {
        base.Show();
        exitButton.Select();
    }
}
