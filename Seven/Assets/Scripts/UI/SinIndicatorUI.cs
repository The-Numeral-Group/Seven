using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Used for displaying sins.
public class SinIndicatorUI : BaseUI
{
    //expects 3 buttons
    [SerializeField]
    List<Button> sinIndicators;
    //expects 3 values
    [SerializeField]
    List<int> sinFlagIndices;
    public GameSaveManager gameSaveManager;
    
    void Start()
    {
        if (sinFlagIndices.Count != sinIndicators.Count)
        if (gameSaveManager)
        {
            DisplaySin();
        }
        else
        {
            var gsm = GameObject.FindObjectOfType<GameSaveManager>();
            if (gsm)
            {
                gameSaveManager = gsm.GetComponent<GameSaveManager>();
                DisplaySin();
            }
        }
    }

    public void DisplaySin()
    {
        for (int i = 0; i < sinFlagIndices.Count; i++)
        {
            if (gameSaveManager.getBoolValue(sinFlagIndices[i]))
            {
                sinIndicators[i].gameObject.SetActive(true);
            }
        }
    }
}
