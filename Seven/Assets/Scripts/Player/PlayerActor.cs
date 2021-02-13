using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerActor : Actor
{
    //Flag to notify if the player is talking with another actor
    public bool isTalking { get; set; }
    //Reference to the PlayerInput cokponent for input map swapping.
    [HideInInspector]
    public PlayerInput playerInput;
    //used for helping swap menus.
    string inputMapString = "UI";

    //Initialize non monobehaviour fields
    void Awake()
    {
        isTalking = false;
    }

    //Initialize monobehaviour fields
    protected override void Start()
    {
        base.Start();
        //Setting to true for testing
        this.myHealth.vulnerable = true;
        playerInput = GetComponent<PlayerInput>();
    }

    /*Engages the dialogue sequence. Disables the players health component, and sets its
    movement direction to 0. Essentially locks the player in place and makes them invulnerable.
    Requires ActiveSpeaker to have been set.*/
    public void StartTalking()
    {
        if (isTalking)
        {
            Debug.LogWarning("Player Actor: Player is already talking.");
            return;
        }
        MenuManager.StartDialogue();
        this.isTalking = true;
        playerInput.SwitchCurrentActionMap("UI");
        this.myMovement.MoveActor(Vector2.zero);
        this.myHealth.enabled = false;

        //MenuManager.DIALOGUE_MENU.speakerNameTextBox.text = ActiveSpeaker.ACTIVE_NPC.speakerName;
        ActiveSpeaker.ACTIVE_NPC.SetIsTalking(true);
    }

    void OnMenu()
    {
        this.myMovement.MoveActor(Vector2.zero);
        playerInput.SwitchCurrentActionMap(inputMapString);
        if (inputMapString == "UI")
        {
            inputMapString = "Player";
        }
        else
        {
            inputMapString = "UI";
        }
        MenuManager.StartPauseMenu();
    }
}
