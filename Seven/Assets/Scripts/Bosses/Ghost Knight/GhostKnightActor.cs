﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
GhostKnightActor's main function is to run the State Machine that powers
Ghost Knight's boss fight
*/
public class GhostKnightActor : Actor
{

    [Tooltip("Indicates which nth attack Ghost Knight performs the special attack")]
    public int specialAttackGate = 7;

    [Tooltip("How close the player should be before Ghost Knight tries to slash them")]
    public float slashRange = 25f;

    [Tooltip("Chance of picking projectile attack compared to slash attack. (1 = 1:1, 2 = 1:2, and so on)")]
    public float projectileRatio = 3f;

    private int specialAttackCounter = 1;

    private Actor ghostKnight;
    private Actor player;

    public State currentState;
    private ActorAbility currAbility;

    private ActorAbility slash;
    private ActorAbility proj;
    private ActorAbility special;

    ///private GhostKnightSlash slash;
    //private GhostKnightProjectile projectile;
    //private GhostKnightSpecial special;

    public enum State
    {
        WAITING,
        WALK,
        PHYSICAL_SLASH,
        LAUNCH_PROJECTILE,
        SPECIAL,
        NULL,
    }

    
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];

        if (playerObject == null)
        {
            Debug.LogWarning("GhostKnightActor: Ghost Knight can't find the player!");
        }
        else
        {
            player = playerObject.GetComponent<Actor>();
        }

        slash = this.myAbilityInitiator.abilities[AbilityRegister.GHOSTKNIGHT_SLASH];
        proj = this.myAbilityInitiator.abilities[AbilityRegister.GHOSTKNIGHT_PROJECTILE];
        special = this.myAbilityInitiator.abilities[AbilityRegister.GHOSTKNIGHT_SPECIAL];

        ghostKnight = this.gameObject.GetComponent<GhostKnightActor>();
        ghostKnight.myHealth.vulnerable = true;
        currentState = State.WAITING;
        currAbility = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currAbility != null)
        {
            checkIfAbilityDone();
        }
        if ((currentState == State.WAITING) || (currentState == State.WALK))
        {
            decideNextState();
            EvaluateState(currentState);
        }
  
    }

    void EvaluateState(State state)
    {
        switch (state)
        {
            case State.WALK:
                stepTowardsPlayer();            
                break;

            case State.PHYSICAL_SLASH:

                slash.Invoke(ref ghostKnight);
                specialAttackCounter++;

                break;

            case State.LAUNCH_PROJECTILE:

                proj.Invoke(ref ghostKnight);
                specialAttackCounter++;

                break;

            case State.SPECIAL:

                special.Invoke(ref ghostKnight);

                break;

            case State.NULL:
                break;

            default:
                Debug.LogWarning("GhostKnightActor: Ghost Knight has fallen out of its state machine!");
                break;
        }
    }
    void stepTowardsPlayer()
    {
        var myPos = this.gameObject.transform.position;
        var playerPos = player.gameObject.transform.position;

        var directionToPlayer = (playerPos - myPos).normalized;

        this.myMovement.MoveActor(directionToPlayer);
    }
    void checkIfAbilityDone()
    {
        // If the currAbility has finished, reset.
        if (currAbility.getIsFinished())
        {
            currAbility = null;
            currentState = State.WAITING;
        }
    }
    void decideNextState()
    {
        var distanceToPlayer = Vector2.Distance(player.transform.position, this.gameObject.transform.position);

        bool slashReady = slash.getUsable(), projReady = proj.getUsable(), specialReady = special.getUsable();

        // Determines which attack the ghost knight will perform.
         int whichAtt = (int)Random.Range(1, projectileRatio + 2);

        // check for special attack counter.
        // if it is 7, activate special attack. 
        if ((specialAttackCounter >= specialAttackGate) && specialReady)
        {
            specialAttackCounter = 1;
            currentState = State.SPECIAL;
            currAbility = special;
        }
        else if ((whichAtt == 1) && projReady)
        {
            currentState = State.LAUNCH_PROJECTILE;
            currAbility = proj;
        }
        else if ((whichAtt != 1) && (distanceToPlayer <= slashRange) && slashReady)
        {
            currentState = State.PHYSICAL_SLASH;
            currAbility = slash;
        }
        else
        {
            currentState = State.WALK;
        }

    }
}
