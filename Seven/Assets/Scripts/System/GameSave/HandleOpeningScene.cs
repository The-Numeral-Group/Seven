using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleOpeningScene : MonoBehaviour
{
    public GameObject gameSaveManager;

    public int bossID;
    
    // Start is called before the first frame update
    void Start()
    {
        this.gameSaveManager.GetComponent<GameSaveManager>().setBoolValue(false, 16);
    }

}
