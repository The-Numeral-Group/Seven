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
        this.newGame = gameSaveList.getBoolValue(0);
        this.playerCurrentScene = gameSaveList.getStringValue(1);

        this.playerPosition = new float[2];
        this.playerPosition[0] = gameSaveList.getVectorValue(2).x;
        this.playerPosition[1] = gameSaveList.getVectorValue(2).y;

        this.ghostKnightPosition = new float[2];
        this.ghostKnightPosition[0] = gameSaveList.getVectorValue(3).x;
        this.ghostKnightPosition[1] = gameSaveList.getVectorValue(3).y;

        this.ApathyDefeated = gameSaveList.getBoolValue(10);
        this.DesireDefeated = gameSaveList.getBoolValue(11);
        this.EgoDefeated = gameSaveList.getBoolValue(12);
        this.IndulgenceDefeated = gameSaveList.getBoolValue(13);
    }

}
