using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

// Source: https://www.youtube.com/watch?v=7ujN52_dTjk&list=PL4vbr3u7UKWp0iM1WIfRjCDTI03u43Zfu&index=69&ab_channel=MisterTaftCreates
// GameSaveManager allows to save certain values and conditions between scenes.
// For example, if you want to make players stand on same position after scene transition, 
// You can save the player's position and use that to place the player after the scene transition.
// Doc: https://docs.google.com/document/d/1of19f71D2yKrfy_kZ0q6iqCK5W_lrzfAmRAalqbgDhk/edit
public class GameSaveManager : MonoBehaviour
{
    public GameObject gameSaveListObject;
    private GameSaveList gameSaveList;

    public bool saveCurrentScene;

    public PlaceObject[] placeObjects;

    private void Start()
    {
        this.gameSaveList = gameSaveListObject.GetComponent<GameSaveList>();
        if (saveCurrentScene)
        {
            this.setNewScene(SceneManager.GetActiveScene().name, 1);
        }
        if (placeObjects.Length > 0)
        {
            foreach (PlaceObject pO in placeObjects)
            {
                Vector2 newPos = this.gameSaveList.getPosition(pO.id);
                pO.gameObject.position = newPos;
            }
        }
    }

    private void OnEnable()
    {
        LoadSaveList();
    }

    // Reset all the SaveList.
    public void ResetSaveList()
    {
        // Set newGame to false
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

        // Convert gameSaveList to json
        var json = JsonUtility.ToJson(gameSaveList);

        // Serialize the json using binary formatter
        binary.Serialize(file, json);
        file.Close();

        /*for (int i = 0; i < SaveObjects.Count; i++)
        {
            // Create a file called i.dat for each object. (0.dat, 1.dat, 2.dat, and so on)
            FileStream file = File.Create(Application.persistentDataPath +
                string.Format("/{0}.dat", i));

            BinaryFormatter binary = new BinaryFormatter();

            // Convert objects[i] tp json
            var json = JsonUtility.ToJson(SaveObjects[i]);

            // Serialize the json using binary formatter
            binary.Serialize(file, json);
            file.Close();
        }*/
    }

    
    public void LoadSaveList()
    {
        // Check if the File exists
        if (File.Exists(Application.persistentDataPath + ("/SaveFile.dat")))
        {
            // Open the file
            FileStream file = File.Open(Application.persistentDataPath + ("/SaveFile.dat"), FileMode.Open);

            BinaryFormatter binary = new BinaryFormatter();

            // Take the file -> deserialize it using the binary formatter 
            // -> Convert it to string -> Override that to gameSaveList
            JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), gameSaveList);
            file.Close();
        }

        /*for (int i = 0; i < SaveObjects.Count; i++)
        {
            // Check if the File exists
            if (File.Exists(Application.persistentDataPath +
                string.Format("/{0}.dat", i)))
            {
                // Open the file
                FileStream file = File.Open(Application.persistentDataPath +
                    string.Format("/{0}.dat", i), FileMode.Open);

                BinaryFormatter binary = new BinaryFormatter();

                // Take the file -> deserialize it using the binary formatter 
                // -> Convert it to string -> Override that to objects[i]
                JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), SaveObjects[i]);
                file.Close();
            }
        }*/
    }
    
    public void setNewGame(bool value)
    {
        this.gameSaveList.setNewGame(value);
        this.SaveSaveList();
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
        if(!this.gameSaveList.getNewGame())
        {
            string currentScene = this.gameSaveList.getCurrentScene(id);
            if (currentScene != "")
            {
                SceneManager.LoadScene(currentScene);
            }
        }
    }

}

[System.Serializable]
public class PlaceObject
{
    public Transform gameObject;

    public int id;
}