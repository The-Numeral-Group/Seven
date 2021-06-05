using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

//sub menu used to handle displaying ability information text
public class AbilitySubMenu : SubMenu
{
    public string lockedAbilityText = "Ability not yet acquired.";
    [TextArea]
    public string indulgenceTODText = "";
    [TextArea]
    public string indulgenceAOSText = "";
    [TextArea]
    public string egoTODText = "";
    [TextArea]
    public string egoAOSText = "";
    [TextArea]
    public string apathyTODText = "";
    [TextArea]
    public string apathyAOSText = "";
    public Text infoBox;
    public Button indulgenceButton;
    public Button egoButton;
    public Button apathyButton;
    public GameSaveManager gameSaveManager;
    Dictionary<string, (string, string)> abilityInfo;
    Dictionary<string, (int, int)> abilityFlag;

    //gamesaveindices 7, 8, 9 ability pickup :: 17, 14, 11 sin committed

    void Awake()
    {
        infoBox.text = "";
        abilityInfo = new Dictionary<string, (string, string)>();
        abilityFlag = new Dictionary<string, (int, int)>();
        abilityInfo.Add("Indulgence", (indulgenceTODText, indulgenceAOSText));
        abilityInfo.Add("Ego", (egoTODText, egoAOSText));
        abilityInfo.Add("Apathy", (apathyTODText, apathyAOSText));
        abilityFlag.Add("Indulgence", (7, 17));
        abilityFlag.Add("Ego", (8, 14));
        abilityFlag.Add("Apathy", (9, 11));
    }

    void Start()
    {
        if (gameSaveManager == null)
        {
            var gsm = GameObject.FindObjectOfType<GameSaveManager>();
            if (gsm)
            {
                gameSaveManager = gsm.GetComponent<GameSaveManager>();
            }
        }
    }

    public void UpdateAbilityText(string bossName)
    {
        if (abilityInfo.ContainsKey(bossName))
        {
            if (gameSaveManager.getBoolValue(abilityFlag[bossName].Item1)) //check if ability picked up
            {
                if (gameSaveManager.getBoolValue(abilityFlag[bossName].Item2)) //check if sin commit
                {
                    infoBox.text = abilityInfo[bossName].Item2;
                }
                else
                {
                    infoBox.text = abilityInfo[bossName].Item1;
                }
            }
            else
            {
                infoBox.text = lockedAbilityText;
            }
        }
        else
        {
            infoBox.text = "";
        }
    }
}
