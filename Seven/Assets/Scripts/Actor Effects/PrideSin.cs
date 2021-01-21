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
    [Tooltip("The amount of interaction presses are needed for the statue to be built.")]
    public int buildsToFinish = 7;

    [Tooltip("The amount of interaction presses that have already been built.")]
    public int currentBuilds = 0;

    [Tooltip("The UI Canvas that displays the interaction prompt when the player is in range.")]
    public GameObject interactUI;

    //whether or not the statue is fully built
    private bool built;

    //whether or not the player is close enough to build the statue
    private bool buildActive;

    //a reference to the player's Actor, for convinience
    private Actor player;

    /*Both of these are references to two of the player's abilities. They both require a class to
    be typed down to a subclass, which is inherently shady. Be ready for weird stuff because of
    this, ESPECIALLY if these abilities are replaced with different kinds of abilities.*/
    private Dodge playerDodge; 
    private WeaponAbility playerAttack;

    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        interactUI.SetActive(false);

        player = GameObject.FindWithTag("Player").GetComponent<Actor>();
        //INSTEAD OF CHANGING THE ABILITIES, REPLACE THEM WITH DIFFERENT ABILITIES
        //SCALE THOSE DIFFERENT ABILITIES WITH CURRENT BUILDS
        //playerDodge = player.myAbilityInitiator.abilities[AbilityRegister.PLAYER_ATTACK];
        //playerAttack = player.myAbilityInitiator.abilities[AbilityRegister.PLAYER_DODGE];
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

    /*Applies the Actor effect. What that means for Pride:
    
    -Increase target size
    -Increase Range of player's attack
    -Increase Size of player's attack
    -Increase Damage of player's attack
    -Decrease Dodge effectiveness (decrease distance, increase lockout time)*/
    public bool ApplyEffect(ref Actor actor)
    {
        return true;
    }

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
        }
    }


}
