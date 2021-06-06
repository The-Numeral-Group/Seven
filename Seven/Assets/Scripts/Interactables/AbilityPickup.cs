using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPickup : Interactable
{
    [Tooltip("Reference to GameSaveManger in scene")]
    public GameSaveManager gameSaveManager;
    [Tooltip("Index of scriptable bool in game save lsit to be notified of ability pickup.")]
    public int gameSaveAbilityPickupIndex = 0;

    [Tooltip("The UI prefab to attach to the player to tutorialize abilities")]
    public GameObject abilityTutorial;

    void Start()
    {
        if (gameSaveManager.getBoolValue(gameSaveAbilityPickupIndex))
        {
            this.gameObject.SetActive(false);
        }
    }
    public override void OnInteract()
    {
        gameSaveManager.setBoolValue(true, gameSaveAbilityPickupIndex);
        MenuManager.ABILITY_MENU.UnlockAbilities();
        Instantiate(abilityTutorial, GameObject.Find("Player").transform);
        this.gameObject.SetActive(false);
    }
}
