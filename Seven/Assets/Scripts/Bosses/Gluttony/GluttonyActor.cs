using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*GluttonyActor's main function is to run the State Machine that powers
Gluttony's boss fight*/
//[RequireComponent(typeof(Effector2D))]
public class GluttonyActor : Actor
{
    [Tooltip("Controls how many times should Gluttony use normal attacks before using it's special.")]
    public int specialAttackGate = 7;

    [Tooltip("How far away the player should be before Gluttony tries to crush them.")]
    public float crushRange = 60f;

    [Tooltip("How close the player should be before Gluttony tries to bite them.")]
    public float biteRange = 25f;

    int specialAttackCounter = 0;

    /*We need to save an additional reference to this script because 'this' is read-only.
    The player reference is just for convinience*/
    Actor gluttony;
    Actor player;
    public State currentState;

    private ActorAbility currAbility;
    public enum State
    {
        WALK,
        PHYSICAL_CRUSH,
        PHYSICAL_BITE,
        PHASE0_SPECIAL,
        LAUNCH_PROJECTILE,
        NULL,
    }

    bool changingPhase = false;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];
        if(playerObject == null)
        {
            Debug.LogWarning("GluttonyActor: Gluttony can't find the player!");
        }
        else
        {
            player = playerObject.GetComponent<Actor>();
        }

        gluttony = this.gameObject.GetComponent<GluttonyActor>();
        gluttony.myHealth.vulnerable = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //might need to move this check outside of fixed update;
        if (this.myHealth.currentHealth <= 5 && !changingPhase)
        {
            changingPhase = true;
            System.Tuple<Actor, System.Action<Actor>> p2 = 
                new System.Tuple<Actor, System.Action<Actor>>(gluttony, null);
            gameObject.SendMessage("NextPhase", p2);
        }
        EvaluateState(currentState);
    }

    void EvaluateState(State state)
    {
        switch (state)
        {
            //Moved the functions in update to walk to avoid the stuttering that occurs on ground pound.
            case State.WALK:
                
                if (currAbility && !currAbility.getIsFinished())
                {
                    break;
                } 
                else if (specialAttackCounter >= specialAttackGate)
                {
                    // check for special attack counter.
                    // if it is 7, activate special attack.
                    specialAttackCounter = 0;
                    currentState = State.PHASE0_SPECIAL;
                }
                else
                {
                    stepTowardsPlayer();
                    currAbility = null;
                    currentState = decideNextState();
                }
                break;

            case State.PHYSICAL_CRUSH:
                /*100% trusting that the Crush ability will handle the entire Crush
                Semirelated, but Gluttony will try to crush 100% of the time if the player is
                far enough. If Crush isn't open at the time, Gluttony will just keep walking*/
                var crush = this.myAbilityInitiator.abilities[AbilityRegister.GLUTTONY_CRUSH];
                currAbility = crush;
                crush.Invoke(ref gluttony, player);
                specialAttackCounter++;

                currentState = State.WALK;
                break;

            case State.PHYSICAL_BITE:
                /*Bite hasn't been implemented yet, but this will probably use a
                "If usable invoke if not just walk" check similar to crush and special*/
                var bite = this.myAbilityInitiator.abilities[AbilityRegister.GLUTTONY_BITE];
                Debug.Log("In Bite");
                currAbility = bite;
                bite.Invoke(ref gluttony, player);
                specialAttackCounter++;

                currentState = State.WALK;
                break;

            case State.PHASE0_SPECIAL:
                var special = this.myAbilityInitiator.abilities[AbilityRegister.GLUTTONY_PHASEZERO_SPECIAL];
                Debug.Log("In Special Ability");
                currAbility = special;
                special.Invoke(ref gluttony, player);

                currentState = State.WALK;
                break;

            case State.LAUNCH_PROJECTILE:
                var proj = this.myAbilityInitiator.abilities[AbilityRegister.GLUTTONY_PROJECTILE];
                Debug.Log("In Projectile");
                currAbility = proj;
                proj.Invoke(ref gluttony);
                specialAttackCounter++;

                currentState = State.WALK;
                break;

            case State.NULL:
                break;

            default:
                Debug.LogWarning("GluttonyActor: Gluttony has fallen out of its state machine!");
                break;
        }
    }
    
    void stepTowardsPlayer()
    {
        var myPos = this.gameObject.transform.position;
        var playerPos = player.gameObject.transform.position;

        var directionToPlayer = playerPos - myPos;
        var playerDirection = player.myMovement.movementDirection * player.myMovement.speed;
        var travelDirection = new Vector2(directionToPlayer.x, directionToPlayer.y) + playerDirection;

        this.myMovement.MoveActor(travelDirection.normalized);
    }

    State decideNextState()
    {
        var distanceToPlayer = Vector2.Distance(player.transform.position, this.gameObject.transform.position);
        //this bool will crash the game if GLUTTONY_BITE isn't correctly defined or if there's no bite ability at all
        bool crushReady = this.myAbilityInitiator.abilities[AbilityRegister.GLUTTONY_CRUSH].getUsable();
        bool projReady = this.myAbilityInitiator.abilities[AbilityRegister.GLUTTONY_PROJECTILE].getUsable();
        bool biteReady = this.myAbilityInitiator.abilities[AbilityRegister.GLUTTONY_BITE].getUsable();
        
        State nextState;  

        //var biteExists = this.myAbilityInitiator.abilityDict[AbilityRegister.GLUTTONY_BITE]?.getUsable();
        //bool biteReady = abilities.TryGetValue(AbilityRegister.GLUTTONY_BITE) ? abilities[AbilityRegister.GLUTTONY_BITE].getUsable() : false

        if(distanceToPlayer >= crushRange)
        {
            int weight = Random.Range(0, 3);
            /*I (Ram) am aware below is bad coding practice. It is in place as a temporary measure
            for the time being to have something that can be played around with. The checks are there
            to stop gluttony from swapping states if the move is on cooldown.*/
            if (weight / 2 == 1)
            {
                if (projReady)
                {
                    nextState = State.LAUNCH_PROJECTILE;
                }
                else
                {
                    nextState = State.WALK;
                }
            }
            else
            {
                if (crushReady)
                {
                    nextState = State.PHYSICAL_CRUSH;
                }
                else
                {
                    nextState = State.WALK;
                }
            }
            /*this movement call is unessecary but added just in case there is an instance where
            movement isn't locked for an ability.*/
            this.myMovement.MoveActor(Vector2.zero);
        }
        else if(distanceToPlayer <= biteRange && biteReady)
        {
            nextState = State.PHYSICAL_BITE;
            /*this movement call is unessecary but added just in case there is an instance where
            movement isn't locked for an ability.*/
            this.myMovement.MoveActor(Vector2.zero);
        }
        else
        {
            nextState = State.WALK;
        }

        return nextState;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag != "Player")
        {
            return;
        }
        //bounceEffector.enabled = true;
        var enemyHealth = collider.gameObject.GetComponent<ActorHealth>();

        //or a weakpoint if there's no regular health
        if(enemyHealth == null){collider.gameObject.GetComponent<ActorWeakPoint>();}

        //if the enemy can take damage (if it has an ActorHealth component),
        //hurt them. Do nothing if they can't take damage.
        if(enemyHealth != null){
            enemyHealth.takeDamage(1f);
        }
    }
}
