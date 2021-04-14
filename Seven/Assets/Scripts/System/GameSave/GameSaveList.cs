using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSaveList : MonoBehaviour
{
    public List<ScriptableObject> SaveObjects = new List<ScriptableObject>();

    // Element 0 - newGame
    //private BoolValue newGame;

    // Element 1 - playerCurrentScene
    //private StringValue playerCurrentScene;

    // Element 2 - playerPosition
    //private VectorValue playerPosition;

    // Element 3 - ghostKnightPosition
    //private VectorValue ghostKnightPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        /*this.newGame = (BoolValue)SaveObjects[0];
        this.playerCurrentScene = (StringValue)SaveObjects[1];
        this.playerPosition = (VectorValue)SaveObjects[2];
        this.ghostKnightPosition = (VectorValue)SaveObjects[3];*/
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setNewGame(bool value)
    {
        ((BoolValue)SaveObjects[0]).RuntimeValue = value;
    }

    public bool getNewGame()
    {
        return ((BoolValue)SaveObjects[0]).RuntimeValue;
    }

    public void setNewPosition(Vector2 newPos, int id)
    {
        if (id >= SaveObjects.Count)
        {
            Debug.LogWarning("The following id: " + id + " in setNewPosition is out of bounds!");
            return;
        }
        if (!(SaveObjects[id] is VectorValue))
        {
            Debug.LogWarning("Element of SaveObjects[" + id + "] is not VectorValue!");
            return;
        }
        ((VectorValue)SaveObjects[id]).RuntimeValue = newPos;
    }

    public Vector2 getPosition(int id)
    {
        if (id >= SaveObjects.Count)
        {
            Debug.LogWarning("The following id: " + id + " in setNewPosition is out of bounds!");
            return Vector2.zero;
        }
        if (!(SaveObjects[id] is VectorValue))
        {
            Debug.LogWarning("Element of SaveObjects[" + id + "] is not VectorValue!");
            return Vector2.zero;
        }
        return ((VectorValue)SaveObjects[id]).RuntimeValue;
    }

    public void setNewScene(string newScene, int id)
    {
        if (id >= SaveObjects.Count)
        {
            Debug.LogWarning("The following id: " + id + " in setNewScene is out of bounds!");
            return;
        }
        if (!(SaveObjects[id] is StringValue))
        {
            Debug.LogWarning("Element of SaveObjects[" + id + "] is not StringValue!");
            return;
        }
        ((StringValue)SaveObjects[id]).RuntimeValue = newScene;
    }

    public string getCurrentScene(int id)
    {
        if (id >= SaveObjects.Count)
        {
            Debug.LogWarning("The following id: " + id + " in getCurrentScene is out of bounds!");
            return "";
        }
        if (!(SaveObjects[id] is StringValue))
        {
            Debug.LogWarning("Element of SaveObjects[" + id + "] is not StringValue!");
            return "";
        }
        return ((StringValue)SaveObjects[id]).RuntimeValue;
    }
}
