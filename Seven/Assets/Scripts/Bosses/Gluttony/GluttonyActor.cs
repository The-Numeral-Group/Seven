using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
GluttonyActor's main function is to run the State Machine that powers
Gluttony's boss fight
*/
public class GluttonyActor : Actor
{

    [Tooltip("When Gluttony's health is lower or equal than this, it's start launching projectiles.")]
    public float projectileHealthGate = 5.0f;

    [Tooltip("Controls how many times should Gluttony use normal attacks before using it's special.")]
    public int specialAttackGate = 7;

    [Tooltip("How far away the player should be before Gluttony tries to crush them.")]
    public float crushRange = 60f;

    [Tooltip("How close the player should be before Gluttony tries to bite them.")]
    public float biteRange = 25f;

    private int specialAttackCounter = 0;

    /*We need to save an additional reference to this script because 'this' is read-only.
    The player reference is just for convinience*/
    private Actor gluttony;
    private Actor player;

    public State currentState;
    public enum State
    {
        WALK,
        PHYSICAL_CRUSH,
        PHYSICAL_BITE,
        PHASE0_SPECIAL,
        LAUNCH_PROJECTILE,
        NULL,
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];

        if(playerObject != null)
        {
            Debug.LogWarning("GluttonyActor: Gluttony can't find the player!");
        }
        else
        {
            player = playerObject.GetComponent<Actor>();
        }

        gluttony = this.gameObject.GetComponent<GluttonyActor>(); 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        EvaluateState(currentState);
    }

    void EvaluateState(State state)
    {
        switch (state)
        {
            //Moved the functions in update to walk to avoid the stuttering that occurs on ground pound.
            case State.WALK:
                if(ShouldProjectile())
                {
                    currentState = State.LAUNCH_PROJECTILE;
                }
                
                
                // check for special attack counter.
                // if it is 7, activate special attack. 
                if (specialAttackCounter >= specialAttackGate)
                {
                    specialAttackCounter = 0;
                    currentState = State.PHASE0_SPECIAL;
                }
                else
                {
                    stepTowardsPlayer();
                    
                    currentState = decideNextState();
                }
                break;

            case State.PHYSICAL_CRUSH:
                /*100% trusting that the Crush ability will handle the entire Crush
                Semirelated, but Gluttony will try to crush 100% of the time if the player is
                far enough. If Crush isn't open at the time, Gluttony will just keep walking*/
                var crush = this.myAbilityInitiator.abilities[AbilityRegister.GLUTTONY_CRUSH];

                if(crush.getUsable())
                {
                    crush.Invoke(ref gluttony);
                }

                currentState = State.WALK;

                break;

            //case State.AFTER_CRUSH:
            //    break;

            case State.PHYSICAL_BITE:
                /*Bite hasn't been implemented yet, but this will probably use a
                "If usable invoke if not just walk" check similar to crush and special*/
                var bite = this.myAbilityInitiator.abilities[AbilityRegister.GLUTTONY_BITE];

                if(bite.getUsable())
                {
                    bite.Invoke(ref gluttony);
                }

                currentState = State.WALK;

                
                break;

            //case State.AFTER_BITE:
            //    break;

            //case State.BEFORE_PHASE0_SPECIAL:
            //    StartCoroutine(PhaseOne_SpecialA());
            //    break;

            case State.PHASE0_SPECIAL:
                var special = this.myAbilityInitiator.abilities[AbilityRegister.GLUTTONY_PHASEZERO_SPECIAL];

                if(special.getUsable())
                {
                    special.Invoke(ref gluttony);
                }

                currentState = State.WALK;

                break;

            case State.LAUNCH_PROJECTILE:
                var proj = this.myAbilityInitiator.abilities[AbilityRegister.GLUTTONY_PROJECTILE];

                if(proj.getUsable())
                {
                    proj.Invoke(ref gluttony);
                }

                currentState = State.WALK;

                break;

            case State.NULL:
                break;

            default:
                Debug.LogWarning("GluttonyActor: Gluttony has fallen out of its state machine!");
                break;
        }
    }

    //GetMovementDirection isn't needed, an equivilant exists in ActorMovement
    //Similarly, many coroutines and substates have been migrated to relevant Abilities

    bool ShouldProjectile()
    {
        //get the projectile ability, if it exists
        var projectiles = this.myAbilityInitiator.abilities[AbilityRegister.GLUTTONY_PROJECTILE];

        //if it doesn't exist, don't shoot it
        if(projectiles == null){return false;}

        //if the projectiles are off cooldown and Gluttony is under the health gate, return true
        if(this.myHealth.currentHealth <= projectileHealthGate && projectiles.getUsable())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    void stepTowardsPlayer()
    {
        var myPos = this.gameObject.transform.position;
        var playerPos = player.gameObject.transform.position;

        /*Fun Fact! Because positions are vectors, the normalized difference between
        posA (myPos) and posB (playerPos) is the direction from posA to posB.*/
        var directionToPlayer = (myPos - playerPos).normalized;

        this.myMovement.MoveActor(directionToPlayer);
    }

    State decideNextState()
    {
        var distanceToPlayer = Vector2.Distance(player.transform.position, this.gameObject.transform.position);
        //this bool will crash the game if GLUTTONY_BITE isn't correctly defined or if there's no bite ability at all
        bool biteReady = this.myAbilityInitiator.abilities[AbilityRegister.GLUTTONY_BITE].getUsable();
        
        State nextState;  

        //var biteExists = this.myAbilityInitiator.abilityDict[AbilityRegister.GLUTTONY_BITE]?.getUsable();
        //bool biteReady = abilities.TryGetValue(AbilityRegister.GLUTTONY_BITE) ? abilities[AbilityRegister.GLUTTONY_BITE].getUsable() : false

        if(distanceToPlayer >= crushRange)
        {
            nextState = State.PHYSICAL_CRUSH;
        }
        else if(distanceToPlayer <= biteRange && biteReady)
        {
            nextState = State.PHYSICAL_BITE;
        }
        else
        {
            nextState = State.WALK;
        }

        return nextState;
    }
}
