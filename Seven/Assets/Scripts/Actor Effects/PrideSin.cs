using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//We use this to judge when the player's using the interact key. Not ideal, but
//that's life.
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CircleCollider2D))]
public class PrideSin : MonoBehaviour, ActorEffect
{
    //FIELDS---------------------------------------------------------------------------------------
    [Header("Control Variables")]
    [Tooltip("The amount of interaction presses are needed for the statue to be built.")]
    public int buildsToFinish = 3;

    [Tooltip("The amount of interaction presses that have already been built.")]
    public int currentBuilds = 0;

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
    as "make it go a smaller distance and make it lockout for longer".*/
    //[Tooltip()]

    [Header("Other")]
    /*
    [Tooltip("The type of the player's dodge ability script (must be specified ahead of time).")]
    public Type dodgeType;

    [Tooltip("The type of the player's attack ability script (must be specified ahead of time).")]
    public Type attackType;*/

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

    /*Both of these are references to two of the player's abilities. They both require a class to
    be typed down to a subclass, which is inherently shady. Be ready for weird stuff because of
    this, ESPECIALLY if these abilities are replaced with different kinds of abilities.
    
    So, you know, very fragile. Be careful. Might want to talk to design about it...
    
    Just kidding! Design cut it. Currently, PrideSin does nothing.*/
    //private Dodge playerDodge; 
    //private WeaponAbility playerAttack;
    //but we're gonna summon them later, sit tight

    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        interactUI.SetActive(false);

        player = GameObject.FindWithTag("Player").GetComponent<Actor>();

        //again, these will break if the ability types are wrong
        //playerDodge = player.myAbilityInitiator.abilities[AbilityRegister.PLAYER_ATTACK] as Dodge;
        //playerAttack = player.myAbilityInitiator.abilities[AbilityRegister.PLAYER_DODGE] as WeaponAbility;
    }

    // Called when a trigger volume attached to this GameObject is entered
    public void OnTriggerEnter2D()
    {
        ToggleBuildActive(true);
    }


    // Called when a trigger volume attached to this GameObject is exited
    public void OnTriggerExit2D()
    {
        ToggleBuildActive(false);
    }

    // Called when the Player presses the "Interact" button
    public void OnInteract()
    {
        if(buildActive)
        {
            BuildStatue();
        }
    }

    /*Currently, PrideSin does nothing
    
    ---OLD COMMENTS---  
    Applies the Actor effect. What that means for Pride:
    
    -Increase target size
    -Increase Range of player's attack
    -Increase Size of player's attack
    -Increase Damage of player's attack
    -Decrease Dodge effectiveness (decrease distance, increase lockout time)
    
    All of these changes need to be applied on a type-by-type basis. If the types
    of these abilites changes, this code will also need to be adjusted (unless the
    new type is a subtype of WeaponAbility and Dodge)*/
    public bool ApplyEffect(ref Actor actor)
    {
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

        */
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

        if(!built && currentBuilds >= buildsToFinish)
        {
            player.myEffectHandler.AddEffect(this);
            built = true;
            ++PrideSin.finishedBuilds;
        }
    }


}
