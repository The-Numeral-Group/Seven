using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GluttonyP2Actor : Actor
{
    //reference to the user
    Actor gluttony;
    //reference to the player
    Actor player;
    ActorAbility currAbility;
    float specialWeight = 50f;
    public State currentState;
    public enum State
    {
        WALK,
        PROJECTILE,
        SPECIAL,
        NULL,
    }
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
        gluttony.myHealth.vulnerable = true; 
    }

    void FixedUpdate()
    {
        EvaluateState(currentState);
    }

    void EvaluateState(State state)
    {
        switch(state)
        {
            case State.WALK:
                if (currAbility && !currAbility.getIsFinished())
                {
                    break;
                }
                else
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

    void stepTowardsPlayer()
    {
        var myPos = this.gameObject.transform.position;
        var playerPos = player.gameObject.transform.position;

        /*Fun Fact! Because positions are vectors, the normalized difference between
        posA (myPos) and posB (playerPos) is the direction from posA to posB.*/
        var directionToPlayer = (playerPos - myPos).normalized;

        this.myMovement.MoveActor(directionToPlayer);
    }

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
            Debug.Log(specialWeight);
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
            Debug.Log(specialWeight);
        }
        return nextState;
    }

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
