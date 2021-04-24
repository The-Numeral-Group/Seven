using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectActiveSwitch : MonoBehaviour
{
    public GameObject gameObjectTarget;
    public void switchFunction()
    {
        if (this.gameObjectTarget.activeSelf)
        {
            this.gameObjectTarget.SetActive(false);
        }
        else
        {
            this.gameObjectTarget.SetActive(true);
        }
    }
}
