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
                var proj = this.myAbilityInitiator.abilities[AbilityRegister.GLUTTONY_PHASETWO_PROJECTILE];
                currAbility = proj;
                proj.Invoke(ref gluttony);
                currentState = State.WALK;
                break;
            case State.SPECIAL:
                var special = this.myAbilityInitiator.abilities[AbilityRegister.GLUTTONY_PHASETWO_SPECIAL];
                currAbility = special;
                special.Invoke(ref gluttony);
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
            nextState = State.SPECIAL;
        }
        else if (projReady)
        {
            nextState = State.PROJECTILE;
        }
        return nextState;
    }
}
