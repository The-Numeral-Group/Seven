using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    // Element List (ID)
    public bool newGame; // 0
    public string playerCurrentScene; // 1
    public float[] playerPosition; // 2
    public float[] ghostKnightPosition; // 3
    public float[] apathyPosition; // 4
    public float[] egoPosition; // 5
    public float[] indulgencePosition; // 6
    public bool IndulgenceAbilityPickup; // 7
    public bool EgoAbilityPickup; // 8
    public bool ApathyAbilityPickup; // 9
    public bool ApathyOpening; // 10
    public bool ApathySinCorrupted; // 11
    public bool ApathyDefeated; // 12
    public bool EgoOpening; // 13
    public bool EgoSinCorrupted; // 14
    public bool EgoDefeated; // 15
    public bool IndulgenceOpening; // 16
    public bool IndulgenceSinCorrupted; // 17
    public bool IndulgenceDefeated; // 18
    public bool PlayerRespawn; // 19

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

        this.PlayerRespawn = gameSaveList.getBoolValue(19);
    }

}
