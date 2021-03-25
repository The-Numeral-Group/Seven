using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeNewGame : MonoBehaviour
{
    public BoolValue newGame;

    // newGame.initialValue = If player is playing this game for the first time.
    // newGame.RuntimeValue = If player has played this game, (have a save file) but wants to restart.

    public void makeNewGameTrue()
    {
        newGame.RuntimeValue = true;
    }
    public void makeNewGameFalse()
    {
        newGame.RuntimeValue = false;
    }
}
