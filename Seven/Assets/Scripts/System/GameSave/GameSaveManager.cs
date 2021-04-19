using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using Yarn;
using Yarn.Unity;

// Source: https://www.youtube.com/watch?v=7ujN52_dTjk&list=PL4vbr3u7UKWp0iM1WIfRjCDTI03u43Zfu&index=69&ab_channel=MisterTaftCreates
// GameSaveManager allows to save certain values and conditions between scenes.
// For example, if you want to make players stand on same position after scene transition, 
// You can save the player's position and use that to place the player after the scene transition.
// Doc: https://docs.google.com/document/d/1of19f71D2yKrfy_kZ0q6iqCK5W_lrzfAmRAalqbgDhk/edit
public class GameSaveManager : MonoBehaviour
{
    public GameObject gameSaveListObject;
    private GameSaveList gameSaveList;


    public PlaceObject[] placeObjects;

    private void Awake()
    {
        this.gameSaveList = gameSaveListObject.GetComponent<GameSaveList>();

        LoadSaveList();

        if (placeObjects.Length > 0)
        {
            foreach (PlaceObject pO in placeObjects)
            {
                pO.gameObject.position = this.gameSaveList.getPosition(pO.id);
            }
        }
    }

    // Reset all the SaveObjects.
    public void ResetSaveList()
    {
        this.gameSaveList.setNewGame(true);
        if (File.Exists(Application.persistentDataPath + ("/SaveFile.dat")))
        {
            File.Delete(Application.persistentDataPath + ("/SaveFile.dat"));
        }
    }

    public void SaveSaveList()
    {
        FileStream file = File.Create(Application.persistentDataPath + ("/SaveFile.dat"));

        BinaryFormatter binary = new BinaryFormatter();

        GameSaveData gameSaveData = new GameSaveData(this.gameSaveList);

        binary.Serialize(file, gameSaveData);
        file.Close();
    }

    
    public void LoadSaveList()
    {
        // Check if the File exists
        if (File.Exists(Application.persistentDataPath + ("/SaveFile.dat")))
        {
            // Open the file
            FileStream file = File.Open(Application.persistentDataPath + ("/SaveFile.dat"), FileMode.Open);

            BinaryFormatter binary = new BinaryFormatter();

            GameSaveData gameSaveData = binary.Deserialize(file) as GameSaveData;

            gameSaveList.setNewGame(gameSaveData.newGame);
            gameSaveList.setNewScene(gameSaveData.playerCurrentScene, 1);

            Vector2 newPlayerPos = new Vector2(gameSaveData.playerPosition[0], gameSaveData.playerPosition[1]);
            gameSaveList.setNewPosition(newPlayerPos, 2);

            Vector2 newGKPos = new Vector2(gameSaveData.ghostKnightPosition[0], gameSaveData.ghostKnightPosition[1]);
            gameSaveList.setNewPosition(newGKPos, 3);

            gameSaveList.setBossProgress(gameSaveData.ApathyDefeated, 10);
            gameSaveList.setBossProgress(gameSaveData.DesireDefeated, 11);
            gameSaveList.setBossProgress(gameSaveData.EgoDefeated, 12);
            gameSaveList.setBossProgress(gameSaveData.IndulgenceDefeated, 13);

            file.Close();
        }
    }

    public void setNewGame(bool value)
    {
        this.gameSaveList.setNewGame(value);
        this.SaveSaveList();
    }

    public bool getNewGame()
    {
        return(this.gameSaveList.getNewGame());
    }

    public void setNewPosition(Vector2 newPos, int id)
    {
        this.gameSaveList.setNewPosition(newPos, id);
        this.SaveSaveList();
    }

    public void setNewScene(string newScene, int id)
    {
        this.gameSaveList.setNewScene(newScene, id);
        this.SaveSaveList();
    }

    public void loadCurrentScene(int id)
    {
        SceneManager.LoadScene(this.gameSaveList.getCurrentScene(id));
    }

    public void continueGame()
    {
        if (!this.gameSaveList.getNewGame())
        {
            this.loadCurrentScene(1);
        }
    }


    [YarnCommand("saveProgress")]
    public void saveProgress()
    {
        this.gameSaveList.setNewScene("Hub", 1);
        this.gameSaveList.setNewGame(false);
        this.gameSaveList.checkBossProgress();
        this.SaveSaveList();
    }

}

[System.Serializable]
public class PlaceObject
{
    public Transform gameObject;

    public int id;
}
