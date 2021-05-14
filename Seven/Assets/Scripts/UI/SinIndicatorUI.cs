using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Used for displaying sins.
public class SinIndicatorUI : BaseUI
{
    //expects 3 buttons
    [SerializeField]
    List<Button> sinIndicators = null;
    //expects 3 values
    [SerializeField]
    List<int> sinFlagIndices = null;
    public GameSaveManager gameSaveManager;
    
    void Start()
    {
        if (sinFlagIndices.Count != sinIndicators.Count)
        if (gameSaveManager)
        {
            DisplaySinFromSaveFlag();
        }
        else
        {
            var gsm = GameObject.FindObjectOfType<GameSaveManager>();
            if (gsm)
            {
                gameSaveManager = gsm.GetComponent<GameSaveManager>();
                DisplaySinFromSaveFlag();
            }
        }
    }

    void DisplaySinFromSaveFlag()
    {
        for (int i = 0; i < sinFlagIndices.Count; i++)
        {
            if (gameSaveManager.getBoolValue(sinFlagIndices[i]))
            {
                sinIndicators[i].gameObject.SetActive(true);
            }
        }
    }

    public void DisplaySin()
    {
        foreach(Button indicator in sinIndicators)
        {
            if (!indicator.gameObject.activeSelf)
            {
                indicator.gameObject.SetActive(true);
                break;
            }
        }
    }

}
