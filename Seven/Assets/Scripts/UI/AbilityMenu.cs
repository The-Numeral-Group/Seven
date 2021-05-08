using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityMenu : BaseUI
{
    [SerializeField]
    List<AbilityButton> abilityButtons = null;
    int pointerToCurrentSelectedButton;
    public PlayerAbilityInitiator player;
    [SerializeField]
    RectTransform abilityHighLightIndicator;
    List<(int, int)> gameSaveAbilityIndices;
    public GameSaveManager gameSaveManager;

    protected override void Awake()
    {
        base.Awake();
        gameSaveAbilityIndices = new List<(int, int)>();
        pointerToCurrentSelectedButton = 0;
        //First int is if the ability has been picked up, the second is if the sin was comitted
        //Indulgence flags in save manager 
        gameSaveAbilityIndices.Add((7, 17));
        //Ego flags in save Manager
        gameSaveAbilityIndices.Add((8, 14));
        //Apathy flags in save manager
        gameSaveAbilityIndices.Add((9, 11));
    }

    //Fetches player reference
    void Start()
    {
        SetupPlayerReference();
        //hard coded values cause lazy to vix visual bug
        abilityHighLightIndicator.position = abilityButtons[pointerToCurrentSelectedButton].transform.position;
        if (gameSaveManager)
        {
            UnlockAbilities();
        }
        else
        {
            var gsm = GameObject.FindObjectOfType<GameSaveManager>();
            if (gsm)
            {
                gameSaveManager = gsm.GetComponent<GameSaveManager>();
                UnlockAbilities();
            }
        }
        //Temporary setup
        abilityHighLightIndicator.gameObject.SetActive(true);
        abilityButtons[0].gameObject.SetActive(true);
        abilityButtons[1].gameObject.SetActive(true);
        abilityButtons[2].gameObject.SetActive(true);
        abilityButtons[0].SetSelectedAbility(true);
        abilityButtons[1].SetSelectedAbility(false);
        abilityButtons[2].SetSelectedAbility(false);
        UpdatePlayerSelectedAbility();
        //End of temporary
    }

    //Unlocks abilities in ability buttons.
    public void UnlockAbilities()
    {
        int i = 0;
        foreach(var indexTuple in gameSaveAbilityIndices)
        {
            if (gameSaveManager.getBoolValue(indexTuple.Item1))
            {
                abilityHighLightIndicator.gameObject.SetActive(true);
                abilityButtons[i].gameObject.SetActive(true);
                abilityButtons[i].SetSelectedAbility(gameSaveManager.getBoolValue(indexTuple.Item2));
            }
            i++;
        }
    }

    void SetupPlayerReference()
    {
        if (!player)
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject)
            {
                player = playerObject.GetComponent<PlayerAbilityInitiator>();
            }
        }
    }

    public void SelectRightAbility()
    {
        int dummyIndex = pointerToCurrentSelectedButton;
        while (dummyIndex < abilityButtons.Count - 1)
        {
            dummyIndex += 1;
            if (abilityButtons[dummyIndex].gameObject.activeSelf)
            {
                pointerToCurrentSelectedButton = dummyIndex;
                abilityHighLightIndicator.position = abilityButtons[pointerToCurrentSelectedButton].transform.position;
                UpdatePlayerSelectedAbility();
                break;
            }
        }
    }

    public void SelectLeftAbility()
    {
        int dummyIndex = pointerToCurrentSelectedButton;
        while (dummyIndex > 0)
        {

            dummyIndex -= 1;
            if (abilityButtons[dummyIndex].gameObject.activeSelf)
            {
                pointerToCurrentSelectedButton = dummyIndex;
                abilityHighLightIndicator.position = abilityButtons[pointerToCurrentSelectedButton].transform.position;
                UpdatePlayerSelectedAbility();
                break;
            }
        }
    }

    public void PutButtonOnCooldown(float time, Component abilityType)
    {
        Debug.Log(pointerToCurrentSelectedButton);
        if (abilityButtons[pointerToCurrentSelectedButton].gameObject.activeSelf &&
        abilityButtons[pointerToCurrentSelectedButton].GetComponent(abilityType.GetType()) != null)
        {
            abilityButtons[pointerToCurrentSelectedButton].StartCooldown(time);
        }
    }

    void UpdatePlayerSelectedAbility()
    {
        Component abilityType = abilityButtons[pointerToCurrentSelectedButton].selectedAbility;
        if (abilityType != null)
        {
            player.selectedAbility = player.GetComponent(abilityType.GetType()) as ActorAbility;
        }
        else
        {
            player.selectedAbility = null;
        }
    }
}
