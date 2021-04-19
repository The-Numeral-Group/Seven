using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public bool newGame;

    public string playerCurrentScene;

    public float[] playerPosition;

    public float[] ghostKnightPosition;

    public bool ApathyDefeated;

    public bool DesireDefeated;

    public bool EgoDefeated;

    public bool IndulgenceDefeated;

    public GameSaveData(GameSaveList gameSaveList)
    {
        this.newGame = gameSaveList.getNewGame();
        this.playerCurrentScene = gameSaveList.getCurrentScene(1);

        this.playerPosition = new float[2];
        this.playerPosition[0] = gameSaveList.getPosition(2).x;
        this.playerPosition[1] = gameSaveList.getPosition(2).y;

        this.ghostKnightPosition = new float[2];
        this.ghostKnightPosition[0] = gameSaveList.getPosition(3).x;
        this.ghostKnightPosition[1] = gameSaveList.getPosition(3).y;

        this.ApathyDefeated = gameSaveList.getBossProgress(10);
        this.DesireDefeated = gameSaveList.getBossProgress(11);
        this.EgoDefeated = gameSaveList.getBossProgress(12);
        this.IndulgenceDefeated = gameSaveList.getBossProgress(13);
    }

}
