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
        ActiveSpeaker.ACTIVE_NPC = ActiveSpeaker.POTENTIAL_SPEAKER;
        MenuManager.DIALOGUE_MENU.StartDialogue();
    }

    void OnMenu()
    {
        this.myMovement.MoveActor(Vector2.zero);
        //Temporary measure
        if (MenuManager.GAME_IS_OVER)
        {
            return;
        }

        if (playerInput.currentActionMap.name == "UI")
        {
            playerInput.SwitchCurrentActionMap("Player");
        }
        else
        {
            playerInput.SwitchCurrentActionMap("UI");
        }
        bool value = MenuManager.StartPauseMenu();
        if (!value)
        {
            /*WARNING: there is a potential redundancy with this line. a menus close callback function
            should reset the players input map. This was added solely for the case where there is issue
            with regards to menumanagers current menu reference.*/
            playerInput.SwitchCurrentActionMap("Player");
        }
    }

    public override void DoActorDamageEffect(float damage)
    {
        // Play TakeDamage Audio
        mySoundManager.PlaySound("TakeDamage");
    }
}
