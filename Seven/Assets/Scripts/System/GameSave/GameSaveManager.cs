using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using Yarn;
using Yarn.Unity;

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
                pO.gameObject.position = this.gameSaveList.getVectorValue(pO.id);
            }
        }

    }

    // Reset all the SaveObjects.
    public void ResetSaveList()
    {
        this.gameSaveList.setBoolValue(true, 0);
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

            gameSaveList.setBoolValue(gameSaveData.newGame, 0);
            gameSaveList.setStringValue(gameSaveData.playerCurrentScene, 1);

            Vector2 newPlayerPos = new Vector2(gameSaveData.playerPosition[0], gameSaveData.playerPosition[1]);
            gameSaveList.setVectorValue(newPlayerPos, 2);

            Vector2 newGKPos = new Vector2(gameSaveData.ghostKnightPosition[0], gameSaveData.ghostKnightPosition[1]);
            gameSaveList.setVectorValue(newGKPos, 3);

            gameSaveList.setBoolValue(gameSaveData.ApathyDefeated, 10);
            gameSaveList.setBoolValue(gameSaveData.DesireDefeated, 11);
            gameSaveList.setBoolValue(gameSaveData.EgoDefeated, 12);
            gameSaveList.setBoolValue(gameSaveData.IndulgenceDefeated, 13);

            file.Close();
        }
    }

    /* ----- BOOL VALUE ----- */
    public void setBoolValue(bool newValue, int id)
    {
        this.gameSaveList.setBoolValue(newValue, id);
        this.SaveSaveList();
    }

    public bool getBoolValue(int id)
    {
        return (this.gameSaveList.getBoolValue(id));
    }

    /* ----- VECTOR VALUE ----- */
    public void setVectorValue(Vector2 newValue, int id)
    {
        this.gameSaveList.setVectorValue(newValue, id);
        this.SaveSaveList();
    }

    public Vector2 getVectorValue(int id)
    {
        return(this.gameSaveList.getVectorValue(id));
    }

    /* ----- STRING VALUE ----- */
    public void setStringValue(string newValue, int id)
    {
        this.gameSaveList.setStringValue(newValue, id);
        this.SaveSaveList();
    }

    public string getStringValue(int id)
    {
        return (this.gameSaveList.getStringValue(id));
    }

    /* ----- FLOAT VALUE ----- */
    public void setFloatValue(float newValue, int id)
    {
        this.gameSaveList.setFloatValue(newValue, id);
        this.SaveSaveList();
    }

    public float getFloatValue(int id)
    {
        return (this.gameSaveList.getFloatValue(id));
    }

    /* ----- OTHER FUNCTIONS ----- */

    public void loadCurrentScene(int id)
    {
        SceneManager.LoadScene(this.gameSaveList.getStringValue(id));
    }

    public void continueGame()
    {
        if (!this.gameSaveList.getBoolValue(0))
        {
            this.loadCurrentScene(1);
        }
    }


    [YarnCommand("saveProgress")]
    public void saveProgress()
    {
        this.gameSaveList.setStringValue("Hub", 1);
        this.gameSaveList.setBoolValue(false, 0);
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
