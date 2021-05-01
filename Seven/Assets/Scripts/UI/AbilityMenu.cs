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

    protected override void Awake()
    {
        base.Awake();
        pointerToCurrentSelectedButton = 0;
    }

    void Start()
    {
        SetupPlayerReference();
        abilityHighLightIndicator.position = abilityButtons[pointerToCurrentSelectedButton].transform.position;
        //Temporary setup
        abilityButtons[0].SetSelectedAbility(true);
        abilityButtons[1].SetSelectedAbility(true);
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
