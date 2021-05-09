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

    public float[] apathyPosition;

    public float[] egoPosition;

    public float[] indulgencePosition;

    public bool IndulgenceAbilityPickup;

    public bool EgoAbilityPickup;

    public bool ApathyAbilityPickup;

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

        this.apathyPosition = new float[2];
        this.apathyPosition[0] = gameSaveList.getVectorValue(4).x;
        this.apathyPosition[1] = gameSaveList.getVectorValue(4).y;

        this.egoPosition = new float[2];
        this.egoPosition[0] = gameSaveList.getVectorValue(5).x;
        this.egoPosition[1] = gameSaveList.getVectorValue(5).y;

        this.indulgencePosition = new float[2];
        this.indulgencePosition[0] = gameSaveList.getVectorValue(6).x;
        this.indulgencePosition[1] = gameSaveList.getVectorValue(6).y;

        this.IndulgenceAbilityPickup = gameSaveList.getBoolValue(7);
        this.EgoAbilityPickup = gameSaveList.getBoolValue(8);
        this.ApathyAbilityPickup = gameSaveList.getBoolValue(9);

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
