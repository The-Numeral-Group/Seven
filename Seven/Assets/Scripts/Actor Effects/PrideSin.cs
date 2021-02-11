using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CircleCollider2D))]
public class PrideSin : MonoBehaviour, ActorEffect
{
    //FIELDS---------------------------------------------------------------------------------------
    [Header("Control Variables")]
    [Tooltip("The amount of interaction presses are needed for the statue to be built.")]
    public int buildsToFinish = 3;

    [Tooltip("The amount of interaction presses that have already been built.")]
    public int currentBuilds = 0;

    [Header("Other")]
    [Tooltip("The UI Canvas that displays the interaction prompt when the player is in range.")]
    public GameObject interactUI;

    [Tooltip("The amount of statues that the player has fully built.")]
    public static int finishedBuilds = 0;

    //whether or not the statue is fully built
    private bool built;

    //whether or not the player is close enough to build the statue
    private bool buildActive;

    //a reference to the player's Actor, for convinience
    private Actor player;

    //helper variables for the system binding
    //A reference to the function that will be performed when a button is pressed
    private System.Action<InputAction.CallbackContext> actionOnInput;

    //A reference to the button-being-pressed event
    private InputAction interactAction;

    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        //automatically turn off the UI prompt
        interactUI.SetActive(false);

        //get the collider and save its size
        var collider = this.gameObject.GetComponent<CircleCollider2D>();
        var originalRadius = collider.radius;

        //Get a reference to the player and its actor        
        var playerObj = GameObject.FindWithTag("Player");
        player = playerObj.GetComponent<Actor>();

        /*ha Ha aint these some doozys! That's low level assignments for ya! Seriously I (Thomas) 
        am not sure what the high level solution is to making things respond to certain actions
        Maybe we should have made input invoke events instead of send messages? 
        Oh well, lessons learned.*/

        //Creating a new delegate that can interact with the input system. It just calls OnInteract
        actionOnInput = new System.Action<InputAction.CallbackContext>(
            (InputAction.CallbackContext c) => {OnInteract();}
        );

        /*Okay here comes the fun because these lines do, like, 12 things
        Step 1: Get the player's PlayerInput
        Step 2: Get the InputActionAsset associated with PlayerInput
        Step 3: Search that InputActionAsset for the Player ActionMap (the player's controls)
            The 'true' argument means that the game will crash if the ActionMap isn't found*/
        var map = playerObj.GetComponent<PlayerInput>().actions.FindActionMap("Player", true);

        /*Step 4: Search that ActionMap for the "Interact" Action
            The 'true' argument means that the game will crash if the Action isn't found
        Step 5: Save that Action*/
        interactAction = map.FindAction("Attack", true);

        /*Step 6: Add the actionOnInput delegate as an action to call with the InteractAction
        is performed.
        Okay I guess they only does 6 things but whatever.*/
        interactAction.performed += actionOnInput;
    }

    // Called when a trigger volume attached to this GameObject is entered
    public void OnCollisionEnter2D(Collision2D col)
    {
        if(this.enabled && col.gameObject.tag == "Player")
        {
            ToggleBuildActive(true);
        }
    }


    // Called when a trigger volume attached to this GameObject is exited
    public void OnCollisionExit2D(Collision2D col)
    {
        if(this.enabled && col.gameObject.tag == "Player")
        {
            ToggleBuildActive(false);
        }
    }

    // Called when the Player presses the button associated with Interact Action
    public void OnInteract()
    {
        if(buildActive)
        {
            BuildStatue();
        }
    }

    //Currently, PrideSin has no actual effect on the player
    public bool ApplyEffect(ref Actor actor)
    {
        return true;
    }

    //Currently, PrideSin does nothing
    public void RemoveEffect(ref Actor actor)
    {

    }

    // Turns the ability (and prompt) to build the statue on or off
    void ToggleBuildActive(bool status)
    {
        buildActive = status;

        interactUI.SetActive(status);
    }

    // Builds the next part of the statue, and applies PrideSin once the statue is built
    void BuildStatue()
    {
        ++currentBuilds;
        //do visual stuff here
        ///DEGUB
        var sprite = this.gameObject.GetComponent<SpriteRenderer>();
        sprite.color = new Color(sprite.color.r, sprite.color.g + 5, sprite.color.b);
        ///DEGUB

        //once the statue is built...
        if(!built && currentBuilds >= buildsToFinish)
        {
            ///DEBUG
            Debug.Log("PrideSin: Statue fully built!");
            ///DEBUG
            //Apply PrideSin to the player and strenghten it's effect
            player.myEffectHandler.AddEffect(this);
            ++PrideSin.finishedBuilds;

            //Mark the statue as built
            built = true;
            ToggleBuildActive(false);

            //Unbind the build input
            interactAction.performed -= actionOnInput;

            //Turn off the build script
            this.enabled = false;
        }
    }


}


/*
Hi. It's Thomas. PrideSin's effect (make player bigger, slower, and do more damage)
got cut, but I'm keeping the original protocode here, just in case...

    /*
    [Header("Sin Effects")]
    [Tooltip("How much bigger should the target get when they commit this sin.")]
    public float scaleFactor = 1.1f;

    [Tooltip("How much the range of the target's attack should be multiplied by.")]
    public float rangeFactor = 1.1f;

    [Tooltip("How much the damage of the target's attack should be multiplied by.")]
    public float damageFactor = 1.1f;
    */

    /*The design doc just said "decrease dodge effectiveness". I (Thomas) am interpreting this
    as "make it go a smaller distance and make it lockout for longer".*
    //[Tooltip()]

        /*
    [Tooltip("The type of the player's dodge ability script (must be specified ahead of time).")]
    public Type dodgeType;

    [Tooltip("The type of the player's attack ability script (must be specified ahead of time).")]
    public Type attackType;*

    //again, these will break if the ability types are wrong
        //playerDodge = player.myAbilityInitiator.abilities[AbilityRegister.PLAYER_ATTACK] as Dodge;
        //playerAttack = player.myAbilityInitiator.abilities[AbilityRegister.PLAYER_DODGE] as WeaponAbility;


    /*Both of these are references to two of the player's abilities. They both require a class to
    be typed down to a subclass, which is inherently shady. Be ready for weird stuff because of
    this, ESPECIALLY if these abilities are replaced with different kinds of abilities.
    
    So, you know, very fragile. Be careful. Might want to talk to design about it...
    
    Just kidding! Design cut it. Currently, PrideSin does nothing.*
    //private Dodge playerDodge; 
    //private WeaponAbility playerAttack;
    //but we're gonna summon them later, sit tight

            ///make a new child game object with a collider of the same size
        var newCollider = new GameObject("statue subcollider");
        newCollider.AddComponent(typeof(CircleCollider2D));
        newCollider.GetComponent<CircleCollider2D>().radius = originalRadius;

        //parent the new collider to this statue
        newCollider.transform.parent = this.gameObject.transform;*

        //increase the original collider's size and make it a trigger
        //collider.radius *= 2;
        //collider.isTrigger = true;
        

        //PlayerController is the C# script generated by the Player Controller inputactions assets
        //controls = new PlayerController();
    *Currently, PrideSin does nothing
    
    ---OLD COMMENTS---  
    Applies the Actor effect. What that means for Pride:
    
    -Increase target size
    -Increase Range of player's attack
    -Increase Size of player's attack
    -Increase Damage of player's attack
    -Decrease Dodge effectiveness (decrease distance, increase lockout time)
    
    All of these changes need to be applied on a type-by-type basis. If the types
    of these abilites changes, this code will also need to be adjusted (unless the
    new type is a subtype of WeaponAbility and Dodge)*

    /*
        var abl = actor.myAbilityInitiator.abilities;
        attackType attack = abl[AbilityRegister.PLAYER_ATTACK] as attackType;
        dodgeType dodge = abl[AbilityRegister.PLAYER_DODGE] as Dodge;

        if(attack == null || dodge == null)
        {
            Debug.LogWarning("PrideSin: Player's attack or dodge cannot be resolved to a type," + 
                " probably due to a mismatch between PrideSin and PlayerAbilityInitiator." + 
                    " Because of this, they cannot be modified");
            return false;
        }

        //Increase the target's size
        actor.gameObject.GetComponent<Transform>().localScale *= scaleFactor;

        //Increase attack range (for WeaponAbility that's weaponPositionScale)
        attack.weaponPositionScale *= rangeFactor;

        //Increase attack damage (for WeaponAbility that's each value in damagePerHitbox)
        foreach(int damage in attack.damagePerHitbox)
        {
            damage *= damageFactor;
        }

        *
*/