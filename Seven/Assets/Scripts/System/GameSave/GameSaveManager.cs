using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// Source: https://www.youtube.com/watch?v=7ujN52_dTjk&list=PL4vbr3u7UKWp0iM1WIfRjCDTI03u43Zfu&index=69&ab_channel=MisterTaftCreates
// GameSaveManager allows to save certain values and conditions between scenes.
// For example, if you want to make players stand on same position after scene transition, 
// You can save the player's position and use that to place the player after the scene transition.
public class GameSaveManager : MonoBehaviour
{
    // These are the datas that are going to get saved when game is closed
    public List<ScriptableObject> objects = new List<ScriptableObject>();

    // Load the Scriptables when you start playing the game
    private void OnEnable()
    {
        LoadScriptables();
    }

    // Save the Scriptables when you stop playing the game
    private void OnDisable()
    {
        SaveScriptables();
    }

    // Reset all the scriptables. 
    public void ResetScriptables()
    {
        for(int i = 0; i < objects.Count; i++)
        {
            if (File.Exists(Application.persistentDataPath +
                string.Format("/{0}.dat", i)))
            {
                File.Delete(Application.persistentDataPath +
                    string.Format("/{0}.dat", i));
            }
        }
    }

    public void SaveScriptables()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            // Create a file called i.dat for each object. (0.dat, 1.dat, 2.dat, and so on)
            FileStream file = File.Create(Application.persistentDataPath +
                string.Format("/{0}.dat", i));

            BinaryFormatter binary = new BinaryFormatter();

            // Convert objects[i] tp json
            var json = JsonUtility.ToJson(objects[i]);

            // Serialize the json using binary formatter
            binary.Serialize(file, json);
            file.Close();
        }
    }

    public void LoadScriptables()
    {
        for (int i = 0; i < objects.Count; i++)
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
                JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), objects[i]);
                file.Close();
            }
        }
    }
}
