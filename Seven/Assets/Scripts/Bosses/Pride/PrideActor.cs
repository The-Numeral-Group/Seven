using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrideActor : Actor
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("Put every GameObject that the player needs to destroy to kill Pride in this list.")]
    public List<ActorWeakPoint> weakSpots;

    [Tooltip("Controls how many times should Pride use normal attacks before using it's special.")]
    public int specialAttackGate = 7;

    [Tooltip("How far away the player should be before Pride launches" + 
        " a long-range attack at them.")]
    public float waveRange = 60f;

    [Tooltip("How far away the player should be before Pride launches" + 
        " a short-range attack at them.")]
    public float punchRange = 25f;

    [Tooltip("How upscaled (how many times bigger) Pride is at max health (will decrease" +  
        " proportionally based on number of weak spots).")]
    public float maxSize = 10f;

    [Tooltip("How upscaled (how many times bigger) Pride is at minimum health (will decrease" +  
        " proportionally based on number of weak spots).")]
    public float minSize = 5f;

    [Tooltip("How much slower Pride is than the player.")]
    public float speedModifier = -5f;

    [Tooltip("Whether or not Pride should override his default speed with the player's speed" + 
        " but slower.")]
    public bool overrideDefaultSpeed = true;

    [Tooltip("What Pride is currently doing. Change this to make Pride do something.")]
    public State currentState;

    //All of Pride's possible states.
    public enum State
    {
        WALK,
        PHYSICAL_PUNCH,
        PHYSICAL_SHOCKWAVE,
        PHASE0_SPECIAL,
        NULL,
    }

    /*reference to this script. Abilities need a reference to their user to be used, but 'this'
    is read-only, and thus can't used. We need to manually reobtain a writable reference.*/
    private Actor pride;

    //reference to player for movement tracking and convinience
    private Actor player;

    //whatever ability Pride is currently using, if any
    private ActorAbility currAbility;

    //internal counter to track when Pride should use its special
    private int specialAttackCounter = 0;

    //METHODS--------------------------------------------------------------------------------------
    /*Aquires references to needed actor components, then sets Pride's speed based on
    overrideDefaultSpeed and speedModifer*/
    new void Start()
    {
        //get component references
        base.Start();

        //find player...
        var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];

        //and save it's actor, if possible
        if(playerObject == null)
        {
            Debug.LogWarning("PrideActor: Pride can't find the player!");
        }
        else
        {
            player = playerObject.GetComponent<Actor>();
        }

        //save pride's own actor
        pride = this.gameObject.GetComponent<PrideActor>();

        //adjust speed if we (design) want the "almost as fast as player" thing
        if(overrideDefaultSpeed)
        {
            this.myMovement.speed = player.myMovement.speed + speedModifier;
        }

        //ensure that the ActorWeakPoints used to hurt Pride are correctly assigned
        foreach(ActorWeakPoint weakSpot in weakSpots)
        {
            weakSpot.ownerHealth = this.myHealth;
        }
    }

    // FixedUpdate is called once every 60th of a second, regardless of framerate
    void FixedUpdate()
    {
        EvaluateState(currentState);
    }

    /*Pride's state machine. Every time this is called, Pride choses a behvaior based on the
    world around it, ususally its distance to the player. Check each case for full 
    explanation.
    
    And yes, I (Thomas) ripped this from Gluttony because Gluttony and Pride act the same way.
    I (Thomas) feel there's a better way to do ability activity tracking than this, but I don't
    know what it is, so I'll use Ram's method (currAbility checking) for now.*/
    void EvaluateState(State state)
    {
        switch (state)
        {
            //Moves Pride towards the player and decides what it should be doing right now
            case State.WALK:
                //Check if Pride is busy doing an ability, and thus shouldn't act
                if (currAbility && !currAbility.getUsable())
                {
                    break;
                }

                //Check for special attack. Once it's high enough, Pride uses its special.
                if (specialAttackCounter >= specialAttackGate)
                {
                    specialAttackCounter = 0;
                    currentState = State.PHASE0_SPECIAL;
                }
                //If it's not special time, Pride will take a step, then decide what to do next.
                else
                {
                    stepTowardsPlayer();
                    currentState = decideNextState();
                }

                break;

            //Activates Pride's long-range Shockwave Smash or Shattered Visage
            case State.PHYSICAL_SHOCKWAVE:
                var wave = this.myAbilityInitiator.abilities[AbilityRegister.PRIDE_FAR_ATTACK];

                if(wave.getUsable())
                {
                    currAbility = wave;
                    wave.Invoke(ref pride);
                    specialAttackCounter++;
                }

                currentState = State.WALK;
                break;

            //Activates Pride's short-range Big-Ass Punch or Scorned Punch
            case State.PHYSICAL_PUNCH:
                var punch = this.myAbilityInitiator.abilities[AbilityRegister.PRIDE_CLOSE_ATTACK];

                if(punch.getUsable())
                {
                    currAbility = punch;
                    punch.Invoke(ref pride);
                    specialAttackCounter++;
                }

                currentState = State.WALK;
                break;

            //Activates Pride's Special, Vanity Tour or Desperate Assault
            case State.PHASE0_SPECIAL:
                var special = this.myAbilityInitiator.abilities[AbilityRegister.PRIDE_SPECIAL];

                if(special.getUsable())
                {
                    currAbility = special;
                    special.Invoke(ref pride);
                }

                currentState = State.WALK;

                break;

            case State.NULL:
                break;

            default:
                Debug.LogWarning("PrideActor: Pride has fallen out of its state machine!");
                break;
        }
    }

    /*Calculates where the player is, then makes one movement instance towards them.
    
    Yes, this was copied from Gluttony.*/
    void stepTowardsPlayer()
    {
        var myPos = this.gameObject.transform.position;
        var playerPos = player.gameObject.transform.position;

        /*Fun Fact! Because positions are vectors, the normalized difference between
        posB (myPos) and posA (playerPos) is the direction from posA to posB.*/
        var directionToPlayer = (playerPos - myPos).normalized;

        this.myMovement.MoveActor(directionToPlayer);
    }

    /*Returns a state to enter based on ability activation conditions, all of which are judged by
    player distance.
    
    Yes, this was copied from Gluttony.*/
    State decideNextState()
    {
        var distanceToPlayer = Vector2.Distance(player.transform.position, this.gameObject.transform.position);
        
        State nextState;  

        if(distanceToPlayer >= waveRange)
        {
            nextState = State.PHYSICAL_SHOCKWAVE;
        }
        else if(distanceToPlayer <= punchRange)
        {
            nextState = State.PHYSICAL_PUNCH;
        }
        else
        {
            nextState = State.WALK;
        }

        return nextState;
    }
}
