using UnityEngine;
using UnityEngine.InputSystem;

//THIS SCRIPT IS MEANT FOR USE WITH THE PRE TUTORIAL PLAYER.
//DO NOT APPLY TO THE PLAYER OUTSIDE OF THAT SCENE AS  IT MAY LEAD TO UNSTABLE PLAUER BEHAVIOUR.
[RequireComponent(typeof(PlayerInput))]
public class PreTutorialPlayerActor : Actor
{
    //Flag to notify if the player is talking with another actor
    public bool isTalking { get; set; }

    //Reference to the PlayerInput cokponent for input map swapping.
    [HideInInspector]
    public PlayerInput playerInput;

    [Tooltip("Whether or not the player should start the scene with their sword.")]
    public bool startWithSword = true;

    public bool hasSword { get; protected set; }

    //Initialize non monobehaviour fields
    void Awake()
    {
        isTalking = false;
        hasSword = true;
    }

    //Initialize monobehaviour fields
    protected override void Start()
    {
        base.Start();
        //this.myAbilityInitiator = GetComponent<PlayerAbilityInitiator>();
        playerInput = GetComponent<PlayerInput>();
        this.myAnimationHandler.Animator.SetBool("player_equiped", startWithSword);
    }

    public override void DoActorDeath()
    {
        MenuManager.StartGameOver();
        if (MenuManager.BATTLE_UI)
        {
            MenuManager.BATTLE_UI.StopAllAudio();
            MenuManager.BATTLE_UI.Hide();
        }
        this.enabled = false;
        //this.gameObject.SetActive(false);
    }

    /*Engages the dialogue sequence. Disables the players health component, and sets its
    movement direction to 0. Essentially locks the player in place and makes them invulnerable.
    Requires ActiveSpeaker to have been set.*/
    public void StartTalking(ActiveSpeaker speaker)
    {
        if (isTalking)
        {
            Debug.LogWarning("Player Actor: Player is already talking.");
            return;
        }
        ActiveSpeaker.ACTIVE_NPC = speaker;
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

    //https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.PlayerInput.html#UnityEngine_InputSystem_PlayerInput_currentControlScheme
    public void OnControlsChanged()
    {
        //MenuManager.SwapControlUIImages(playerInput.currentControlScheme);
        SwapUIImage[] uiSwappers = Resources.FindObjectsOfTypeAll(typeof(SwapUIImage)) as SwapUIImage[];
        foreach(SwapUIImage uiSwapper in uiSwappers)
        {
            uiSwapper.SwapImage(playerInput.currentControlScheme);
        }
    }


    public override void DoActorDamageEffect(float damage)
    {
        // Play TakeDamage Audio
        base.DoActorDamageEffect(damage);
        mySoundManager.PlaySound("TakeDamage");
    }
}
