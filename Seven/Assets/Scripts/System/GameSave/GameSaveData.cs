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

    public bool ApathyOpening;

    public bool ApathySinCorrupted;

    public bool ApathyDefeated;

    public bool EgoOpening;

    public bool EgoSinCorrupted;

    public bool EgoDefeated;

    public bool IndulgenceOpening;

    public bool IndulgenceSinCorrupted;

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

        this.ApathyOpening = gameSaveList.getBoolValue(10);
        this.ApathySinCorrupted = gameSaveList.getBoolValue(11);
        this.ApathyDefeated = gameSaveList.getBoolValue(12);

        this.EgoOpening = gameSaveList.getBoolValue(13);
        this.EgoSinCorrupted = gameSaveList.getBoolValue(14);
        this.EgoDefeated = gameSaveList.getBoolValue(15);

        this.IndulgenceOpening = gameSaveList.getBoolValue(16);
        this.IndulgenceSinCorrupted = gameSaveList.getBoolValue(17);
        this.IndulgenceDefeated = gameSaveList.getBoolValue(18);
    }

}
