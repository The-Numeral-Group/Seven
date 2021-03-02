using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeNewGame : MonoBehaviour
{
    public BoolValue newGame;

    public void makeNewGameTrue()
    {
        newGame.RuntimeValue = true;
    }
    public void makeNewGameFalse()
    {
        newGame.RuntimeValue = false;
    }
}
