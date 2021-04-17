using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSaveList : MonoBehaviour
{
    public List<ScriptableObject> SaveObjects = new List<ScriptableObject>();

    // Element 0 - newGame

    // Element 1 - playerCurrentScene

    // Element 2 - playerPosition

    // Element 3 - ghostKnightPosition

    // Element 10 - ApathyDefeated

    // Element 11 - DesireDefeated

    // Element 12 - EgoDefeated

    // Element 13 - IndulgenceDefeated



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

    public void checkBossProgress()
    {
        if (((BoolValue)SaveObjects[10]).RuntimeValue)
        {
            Debug.Log("Apathy has been defeated!");
        }
        else
        {
            Debug.Log("Apathy has not been defeated!");
        }

        if (((BoolValue)SaveObjects[11]).RuntimeValue)
        {
            Debug.Log("Desire has been defeated!");
        }
        else
        {
            Debug.Log("Desire has not been defeated!");
        }

        if (((BoolValue)SaveObjects[12]).RuntimeValue)
        {
            Debug.Log("Ego has been defeated!");
        }
        else
        {
            Debug.Log("Ego has not been defeated!");
        }

        if (((BoolValue)SaveObjects[13]).RuntimeValue)
        {
            Debug.Log("Indulgence has been defeated!");
        }
        else
        {
            Debug.Log("Indulgence has not been defeated!");
        }
    }
}
