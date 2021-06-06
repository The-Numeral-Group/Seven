using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

//Document Link: https://docs.google.com/document/d/1K9f7sRQvDX4krcKahgIzJey87R1z7mGWV4xgnYU7qSQ/edit?usp=sharing

//Expected to be attached to the pause menu ui.
public class PauseMenu : BaseUI
{
    //Flag for if the game is paused
    public static bool GAME_IS_PAUSED;
    /*[SerializeField]
    [Tooltip("Reference to the Sub Menu Container")]
    RectTransform subMenuContainer = null;*/
    //I did stupid design for the sub menus. List order matters for this class.
    [SerializeField]
    [Tooltip("List containing the various submenus.")]
    List<SubMenu> subMenus = null;
    [Tooltip("Reference to the settings sub menu")]
    public SettingsSubMenu settings;
    //Index of the current selected sub menu in the list
    int currentSelectedSubMenuIndex = 0;
    /*[SerializeField]
    List<GameObject> abilityButtons = null;
    [SerializeField]
    [Tooltip("Reference to the left button which swaps to the next sub menu.")]
    Button leftSubMenuSelect = null;
    [SerializeField]
    [Tooltip("Reference to the right button which swaps to the next sub menu.")]

    Button rightSubMenuSelect = null;
    [SerializeField]
    [Tooltip("Reference to the image representing abilityOne.")]
    Image abilityOneImage = null;
    [SerializeField]
    [Tooltip("Reference to the image representing abilityTwo")]
    Image abilityTwoImage = null;*/
    //Reference to the player
    Actor playerActor;

    void Start()
    {
        LocatePlayer();
    }

    //Function used to set the reference for the player actor.
    public void LocatePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            playerActor = player.GetComponent<Actor>();
        }
        else
        {
            Debug.Log("PauseMenu: Could not find player object.");
        }
    }

    //Public Function which will pause the game
    public void PauseGame()
    {
        if (!GAME_IS_PAUSED)
        {
            Pause();
        }
        else
        {
            Hide();
        }
    }

    //Paused the game
    void Pause()
    {
        GAME_IS_PAUSED = true;
        Time.timeScale = 0f;
    }
    
    //Overriding base class ui.
    public override void Hide()
    {
        GAME_IS_PAUSED = false;
        Time.timeScale = 1f;
        currentSelectedSubMenuIndex = 0;
        foreach(SubMenu menu in subMenus)
        {
            menu.gameObject.SetActive(false);
        }
        base.Hide();
    }

    //Sets the menu object as active.
    public override void Show()
    {
        base.Show();
        var player = GameObject.FindGameObjectWithTag("Player");
        PlayerInput pInput = player.GetComponent<PlayerInput>();
        pInput.SwitchCurrentActionMap("UI");
        ShowSubMenu(subMenus[currentSelectedSubMenuIndex]);
        PauseGame();
    }

    public void ShowSubMenu(SubMenu menu)
    {
        menu.Show();
    }

    //meant to be called by player upon closing pause menu through resume.
    public void OnResume()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        PlayerInput pInput = player.GetComponent<PlayerInput>();
        pInput.SwitchCurrentActionMap("Player");
        Hide();
    }

    //Callback function used by the pause menu to swap between the ability menu and pause menu.
    /*public void SwampMenu(bool swapRight)
    {
        if (swapRight)
        {
            if (currentSelectedSubMenuIndex < subMenus.Count - 1)
            {
                currentSelectedSubMenuIndex++;
                MoveSubMenu(-1100);
                //temporary brute force code
                rightSubMenuSelect.gameObject.SetActive(false);
                leftSubMenuSelect.gameObject.SetActive(true);
            }
        }
        else
        {
            if (currentSelectedSubMenuIndex > 0)
            {
                currentSelectedSubMenuIndex--;
                MoveSubMenu(1100);
                //temporary brute force code
                leftSubMenuSelect.gameObject.SetActive(false);
                rightSubMenuSelect.gameObject.SetActive(true);
            }
        }
        subMenus[currentSelectedSubMenuIndex].defaultButton.Select();
    }

    //Utilized by SwapMenu function to actually move the menus.
    void MoveSubMenu(float value)
    {
        subMenuContainer.localPosition += new Vector3(value, 0, 0);
    }

    //Resource for checking if a ui button is selected
    //https://answers.unity.com/questions/921720/how-can-i-check-if-a-ui-button-is-selected.html
    /*Function below sets abilities for the player within the menu system. Pass in true for abiltiy 1, false for
    ability2. Right now the call is made from the playeractor script.*/
    /*public void SetAbility(bool setAbilityOne)
    {
        GameObject currentSelectedButton = EventSystem.current.currentSelectedGameObject;
        PlayerAbilityInitiator playerAbilityInitiator = playerActor.myAbilityInitiator as PlayerAbilityInitiator;
        if (!abilityButtons.Contains(currentSelectedButton))
        {
            return;
        }

        if (setAbilityOne)
        {
            SetupPlayerAbilityReference(ref playerAbilityInitiator.selectedAbilityAlpha, abilityOneImage);
        }
        else
        {
            SetupPlayerAbilityReference(ref playerAbilityInitiator.selectedAbilityBeta, abilityTwoImage);
        }

        //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/local-functions
        void SetupPlayerAbilityReference(ref ActorAbility selectedPlayerAbility, Image ptrToAbilityUIElement)
        {
            Component abilityType = currentSelectedButton.GetComponent<ActorAbility>();
            if (abilityType != null)
            {
                ActorAbility abilityToAdd = playerActor.GetComponent(abilityType.GetType()) as ActorAbility;
                if (abilityToAdd == null)
                {
                    playerActor.gameObject.AddComponent(abilityType.GetType());
                    abilityToAdd = playerActor.GetComponent(abilityType.GetType()) as ActorAbility;
                }
                selectedPlayerAbility = abilityToAdd;
                Debug.Log(abilityType.GetType().ToString());
            }
            else
            {
                selectedPlayerAbility = null;
            }
            //This needs to be changed from color to sprite
            ptrToAbilityUIElement.color = currentSelectedButton.GetComponent<Image>().color;
        }

    }*/

    //https://forum.unity.com/threads/passing-a-any-component-as-a-parameter.497218/
    /*this function  assigns an ability to an ability button. When assigning an abiltiy it must 
    be derived from type ActorAbility. The ability button selected will overwrite its previous
    ability with the new one. This is essentially adding the component to the button through code
    instead of through the inspector. The primary use I see for it is when a player kills a boss,
    the events that follow can add the ability to w/e index abiltiy button. This will allow the player
    to assign it as their selected ability in the future.*/
    /*public void SetAbilityToAbilityMenu<TAbility>(int index) where TAbility : UnityEngine.Component, new()
    {
        if (index >= abilityButtons.Count)
        {
            Debug.LogWarning("PauseMenu: SetAbilityToAbilityMenu: Index out of bounds. Must be less" +
            " than: " + abilityButtons.Count);
            return;
        }
        //https://stackoverflow.com/questions/33750893/how-can-i-tell-if-one-object-is-derived-from-a-particular-class
        if (!typeof(TAbility).IsSubclassOf(typeof(ActorAbility)))
        {
            Debug.LogWarning("PauseMenu: SetAbilityToAbilityMenu: abilityToAdd is not a derived" +
            " class of ACtorAbility.");
            return;
        }
        Destroy(abilityButtons[index].GetComponent<ActorAbility>());
        abilityButtons[index].AddComponent<TAbility>();
        //Code to set inspector values
        //code to set sprite of button
    }*/
}
