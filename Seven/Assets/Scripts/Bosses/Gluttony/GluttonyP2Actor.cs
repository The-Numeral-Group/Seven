using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Document Link: https://docs.google.com/document/d/1oQAqGqEiA1VBaeJ1k9FU0FjlLO3lpbSyWUtcjh2YRyE/edit?usp=sharing
[RequireComponent(typeof(GluttonyP1AnimationHandler))]
public class GluttonyP2Actor : Actor
{
    //reference to the user
    Actor gluttony;
    //reference to the player
    Actor player;
    //Pointer to the current ability in use
    ActorAbility currAbility;
    //Initial weight for the special.
    float specialWeight = 50f;
    //The current state the actor is in.
    public State currentState;
    public enum State
    {
        WALK,
        PROJECTILE,
        SPECIAL,
        NULL,
    }

    //Reference to the derived animation handler;
    GluttonyP1AnimationHandler gluttonyAnimHandler;
    //Initiliaze monobehaviour fields.
    protected override void Start()
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

        gluttony = this.gameObject.GetComponent<Actor>();
        gluttonyAnimHandler = this.myAnimationHandler as GluttonyP1AnimationHandler; 
    }

    void FixedUpdate()
    {
        EvaluateState(currentState);
    }

    public override void DoActorDeath()
    {
        this.gameObject.SetActive(false);
    }

    //Function operates as the state machine.
    void EvaluateState(State state)
    {
        switch(state)
        {
            case State.WALK:
                if (currAbility && !currAbility.getIsFinished())
                {
                    break;
                }
                else if (player != null)
                {
                    stepTowardsPlayer();
                    currAbility = null;
                    currentState = decideNextState();
                }
                break;
            case State.PROJECTILE:
                Debug.Log("Phase 2: projectile");
                var proj = this.myAbilityInitiator.abilities[AbilityRegister.GLUTTONY_PHASETWO_PROJECTILE];
                currAbility = proj;
                proj.Invoke(ref gluttony);
                currentState = State.WALK;
                break;
            case State.SPECIAL:
                Debug.Log("Phase 2: special");
                var special = this.myAbilityInitiator.abilities[AbilityRegister.GLUTTONY_PHASETWO_SPECIAL];
                currAbility = special;
                special.Invoke(ref gluttony, player);
                currentState = State.WALK;
                break;
            case State.NULL:
                break;
            default:
                Debug.LogWarning("GluttonyActor: Gluttony has fallen out of its state machine!");
                break;
        }
    }

    /*Moves Gluttony towards the player. It will also read the players movement to give gluttony
    a better path towards the player*/
    void stepTowardsPlayer()
    {
        /*
        var myPos = this.gameObject.transform.position;
        var playerPos = player.gameObject.transform.position;

        var directionToPlayer = (playerPos - myPos).normalized;

        this.myMovement.MoveActor(directionToPlayer);*/
        var myPos = this.gameObject.transform.position;
        var playerPos = player.gameObject.transform.position;

        var directionToPlayer = playerPos - myPos;
        var playerDirection = player.myMovement.movementDirection * player.myMovement.speed;
        var travelDirection = new Vector2(directionToPlayer.x, directionToPlayer.y) + playerDirection;

        this.myMovement.MoveActor(travelDirection.normalized);
        gluttonyAnimHandler.AnimateWalk(true, directionToPlayer);
    }

    /*The state logic for phase 2 gluttony. Uses weights to choose to either use the projectile
    attack or special attack.*/
    State decideNextState()
    {
        bool specialReady = 
            this.myAbilityInitiator.abilities[AbilityRegister.GLUTTONY_PHASETWO_SPECIAL].getUsable();
        bool projReady = 
            this.myAbilityInitiator.abilities[AbilityRegister.GLUTTONY_PHASETWO_PROJECTILE].getUsable();
        State nextState;
        nextState = State.WALK;

        float weight = Random.Range(0.0f, 100f);
        if (weight <= specialWeight && specialReady)
        {
            this.myMovement.MoveActor(Vector2.zero);
            nextState = State.SPECIAL;
            if (specialWeight - 25f >= 0)
            {
                specialWeight -= 25f;
            }
            else
            {
                specialWeight = 0.0f;
            }
            gluttonyAnimHandler.AnimateWalk(false, Vector2.zero);
        }
        else if (projReady)
        {
            this.myMovement.MoveActor(Vector2.zero);
            nextState = State.PROJECTILE;
            if (specialWeight + 25f <= 100)
            {
                specialWeight += 25f;
            }
            else
            {
                specialWeight = 100f;
            }
            gluttonyAnimHandler.AnimateWalk(false, Vector2.zero);
        }
        return nextState;
    }

    //Damages the player if they touch gluttony.
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag != "Player")
        {
            return;
        }
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
