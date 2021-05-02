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
        gameSaveAbilityIndices.Add((15, 14));
        //Ego flags in save Manager
        gameSaveAbilityIndices.Add((15, 14));
        //Apathy flags in save manager
        gameSaveAbilityIndices.Add((15, 14));
    }

    //Fetches player reference
    void Start()
    {
        SetupPlayerReference();
        abilityHighLightIndicator.position = abilityButtons[pointerToCurrentSelectedButton].transform.position;
        if (gameSaveManager)
        {
            UnlockAbilities();
        }
        //Temporary setup
        abilityButtons[0].SetSelectedAbility(true);
        abilityButtons[1].SetSelectedAbility(false);
    }

    //Unlocks abilities in ability buttons.
    public void UnlockAbilities()
    {
        int i = 0;
        foreach(var indexTuple in gameSaveAbilityIndices)
        {
            if (gameSaveManager.getBoolValue(indexTuple.Item1))
            {
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
        if (pointerToCurrentSelectedButton < abilityButtons.Count - 1)
        {
            pointerToCurrentSelectedButton += 1;
            abilityHighLightIndicator.position = abilityButtons[pointerToCurrentSelectedButton].transform.position;
            UpdatePlayerSelectedAbility();
        }
    }

    public void SelectLeftAbility()
    {
        if (pointerToCurrentSelectedButton > 0)
        {
            pointerToCurrentSelectedButton -= 1;
            abilityHighLightIndicator.position = abilityButtons[pointerToCurrentSelectedButton].transform.position;
            UpdatePlayerSelectedAbility();
        }
    }

    void UpdatePlayerSelectedAbility()
    {
        Component abilityType = abilityButtons[pointerToCurrentSelectedButton].selectedAbility;
        if (abilityType != null)
        {
            Debug.Log("Applying ability selection");
            player.selectedAbility = player.GetComponent(abilityType.GetType()) as ActorAbility;
        }
    }
}
