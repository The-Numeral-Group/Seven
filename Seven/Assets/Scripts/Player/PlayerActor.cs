using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerActor : Actor
{
    //Flag to notify if the player is talking with another actor
    public bool isTalking { get; set; }
    //Reference to the PlayerInput cokponent for input map swapping.
    PlayerInput playerInput;

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

    /*Callback from the dialogue system finishing a conversation.
    Players health component is reenabled.*/
    public void OnDialogueEnd()
    {
        this.isTalking = false;
        playerInput.SwitchCurrentActionMap("Player");
        this.myHealth.enabled = true;
    }

    /*Engages the dialogue sequence. Disables the players health component, and sets its
    movement direction to 0. Essentially locks the player in place and makes them invulnerable.*/
    public void StartTalking()
    {
        if (isTalking)
        {
            Debug.LogWarning("Player Actor: Player is already talking.");
            return;
        }
        this.isTalking = true;
        playerInput.SwitchCurrentActionMap("UI");
        this.myMovement.MoveActor(Vector2.zero);
        this.myHealth.enabled = false;

        MenuManager.DIALOGUE_MENU.speakerNameTextBox.text = ActiveSpeaker.ACTIVE_NPC.speakerName;
        MenuManager.DIALOGUE_MENU.dialogueRunner.StartDialogue(ActiveSpeaker.ACTIVE_NPC.yarnStartNode);
    }
}
