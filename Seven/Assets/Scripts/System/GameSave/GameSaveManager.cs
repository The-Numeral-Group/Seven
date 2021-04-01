using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// Source: https://www.youtube.com/watch?v=7ujN52_dTjk&list=PL4vbr3u7UKWp0iM1WIfRjCDTI03u43Zfu&index=69&ab_channel=MisterTaftCreates
// GameSaveManager allows to save certain values and conditions between scenes.
// For example, if you want to make players stand on same position after scene transition, 
// You can save the player's position and use that to place the player after the scene transition.
// Doc: https://docs.google.com/document/d/1of19f71D2yKrfy_kZ0q6iqCK5W_lrzfAmRAalqbgDhk/edit
public class GameSaveManager : MonoBehaviour
{
    // These are the datas that are going to get saved when game is closed
    public List<ScriptableObject> SaveObjects = new List<ScriptableObject>();

    public BoolValue newGame;

    private void OnEnable()
    {
        LoadSaveObjects();
    }

    // Reset all the SaveObjects.
    public void ResetSaveObjects()
    {
        newGame.initialValue = true;
        for (int i = 0; i < SaveObjects.Count; i++)
        {
            if (File.Exists(Application.persistentDataPath +
                string.Format("/{0}.dat", i)))
            {
                File.Delete(Application.persistentDataPath +
                    string.Format("/{0}.dat", i));
            }
        }
    }

    public void SaveSaveObjects()
    {
        for (int i = 0; i < SaveObjects.Count; i++)
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
        }
    }

    
    public void LoadSaveObjects()
    {
        for (int i = 0; i < SaveObjects.Count; i++)
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
        }
    }

}
