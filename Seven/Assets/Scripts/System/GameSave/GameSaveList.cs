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


    /* ----- BOOL VALUE ----- */
    public void setBoolValue(bool newValue, int id)
    {
        if (id >= SaveObjects.Count)
        {
            Debug.LogWarning("The following id: " + id + " in setBoolValue is out of bounds!");
            return;
        }
        if (!(SaveObjects[id] is BoolValue))
        {
            Debug.LogWarning("Element of SaveObjects[" + id + "] is not BoolValue!");
            return;
        }
        ((BoolValue)SaveObjects[id]).RuntimeValue = newValue;
    }

    public bool getBoolValue(int id)
    {
        if (id >= SaveObjects.Count)
        {
            Debug.LogWarning("The following id: " + id + " in getBoolValue is out of bounds!");
            return false;
        }
        if (!(SaveObjects[id] is BoolValue))
        {
            Debug.LogWarning("Element of SaveObjects[" + id + "] is not BoolValue!");
            return false;
        }
        return ((BoolValue)SaveObjects[id]).RuntimeValue;
    }

    /* ----- VECTOR VALUE ----- */
    public void setVectorValue(Vector2 newValue, int id)
    {
        if (id >= SaveObjects.Count)
        {
            Debug.LogWarning("The following id: " + id + " in setVectorValue is out of bounds!");
            return;
        }
        if (!(SaveObjects[id] is VectorValue))
        {
            Debug.LogWarning("Element of SaveObjects[" + id + "] is not VectorValue!");
            return;
        }
        ((VectorValue)SaveObjects[id]).RuntimeValue = newValue;
    }

    public Vector2 getVectorValue(int id)
    {
        if (id >= SaveObjects.Count)
        {
            Debug.LogWarning("The following id: " + id + " in getVectorValue is out of bounds!");
            return Vector2.zero;
        }
        if (!(SaveObjects[id] is VectorValue))
        {
            Debug.LogWarning("Element of SaveObjects[" + id + "] is not VectorValue!");
            return Vector2.zero;
        }
        return ((VectorValue)SaveObjects[id]).RuntimeValue;
    }

    /* ----- STRING VALUE ----- */
    public void setStringValue(string newValue, int id)
    {
        if (id >= SaveObjects.Count)
        {
            Debug.LogWarning("The following id: " + id + " in setStringValue is out of bounds!");
            return;
        }
        if (!(SaveObjects[id] is StringValue))
        {
            Debug.LogWarning("Element of SaveObjects[" + id + "] is not StringValue!");
            return;
        }
        ((StringValue)SaveObjects[id]).RuntimeValue = newValue;
    }

    public string getStringValue(int id)
    {
        if (id >= SaveObjects.Count)
        {
            Debug.LogWarning("The following id: " + id + " in getStringValue is out of bounds!");
            return "";
        }
        if (!(SaveObjects[id] is StringValue))
        {
            Debug.LogWarning("Element of SaveObjects[" + id + "] is not StringValue!");
            return "";
        }
        return ((StringValue)SaveObjects[id]).RuntimeValue;
    }

    /* ----- FLOAT VALUE ----- */
    public void setFloatValue(float newValue, int id)
    {
        if (id >= SaveObjects.Count)
        {
            Debug.LogWarning("The following id: " + id + " in setFloatValue is out of bounds!");
            return;
        }
        if (!(SaveObjects[id] is FloatValue))
        {
            Debug.LogWarning("Element of SaveObjects[" + id + "] is not FloatValue!");
            return;
        }
        ((FloatValue)SaveObjects[id]).RuntimeValue = newValue;
    }

    public float getFloatValue(int id)
    {
        if (id >= SaveObjects.Count)
        {
            Debug.LogWarning("The following id: " + id + " in getFloatValue is out of bounds!");
            return 0.0f;
        }
        if (!(SaveObjects[id] is FloatValue))
        {
            Debug.LogWarning("Element of SaveObjects[" + id + "] is not FloatValue!");
            return 0.0f;
        }
        return ((FloatValue)SaveObjects[id]).RuntimeValue;
    }

    /* ----- OTHER FUNCTIONS ----- */

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
