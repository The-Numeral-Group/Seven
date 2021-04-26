using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor.Animations;

[RequireComponent(typeof(PlayerInput))]
public class PlayerActor : Actor
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
        this.myAbilityInitiator = GetComponent<PlayerAbilityInitiator>();
        playerInput = GetComponent<PlayerInput>();

        SetSwordState(startWithSword);
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

    /*Equips (true) or unequips (false) the player's sword by telling its animation handler
    to swap between sworded and swordless animations. Also sets a flag that tells other 
    scripts about the sword's status*/
    public void SetSwordState(bool hasSword)
    {
        this.myAnimationHandler.Animator.SetBool("player_equiped", hasSword);
        this.hasSword = hasSword;

        //check each ability the player has
        foreach(ActorAbility ability in this.myAbilityInitiator.abilities.Values)
        {
            //if it needs the sword
            if(ability is PlayerSwordAbility)
            {
                //and the player has lost the sword
                if(hasSword == false)
                {
                    //lock it forever
                    StartCoroutine(ability.coolDown(Mathf.Infinity));
                }
                //and the player has gained the sword
                else
                {
                    //immediately make it usable
                    StartCoroutine(ability.coolDown(0f));
                }
            }
        }
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
    
    void OnSetAbilityOne()
    {
        if (MenuManager.CURRENT_MENU && MenuManager.CURRENT_MENU.GetType().ToString() == "PauseMenu")
        {
            PauseMenu pMenu = MenuManager.CURRENT_MENU as PauseMenu;
            pMenu.SetAbility(true);
        }
    }

    void OnSetAbilityTwo()
    {
        if (MenuManager.CURRENT_MENU && MenuManager.CURRENT_MENU.GetType().ToString() == "PauseMenu")
        {
            PauseMenu pMenu = MenuManager.CURRENT_MENU as PauseMenu;
            pMenu.SetAbility(false);
        }
    }

    public override void DoActorDamageEffect(float damage)
    {
        // Play TakeDamage Audio
        base.DoActorDamageEffect(damage);
        mySoundManager.PlaySound("TakeDamage");
    }
}
