using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPickup : Interactable
{
    [Tooltip("Reference to GameSaveManger in scene")]
    public GameSaveManager gameSaveManager;
    [Tooltip("Index of scriptable bool in game save lsit to be notified of ability pickup.")]
    public int gameSaveAbilityPickupIndex = 0;
    public override void OnInteract()
    {
        gameSaveManager.setBoolValue(true, gameSaveAbilityPickupIndex);
        MenuManager.ABILITY_MENU.UnlockAbilities();
        this.gameObject.SetActive(false);
    }
}
