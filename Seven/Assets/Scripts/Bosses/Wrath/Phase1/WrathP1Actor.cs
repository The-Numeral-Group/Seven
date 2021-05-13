﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathP1Actor : Actor
{
    public State currentState;

    private Actor wrath;
    private Actor player;

    private ActorAbility currAbility;

    private ActorAbility chainPull;
    private ActorAbility fireWall;
    private ActorAbility sludge;
    private ActorAbility swordAttack;
    private ActorAbility swordRush;

    private char poolType = 'A';
    private bool canAttack = true;

    public enum State
    {
        WALK,
        ABILITY_CHAINPULL,
        ABILITY_FIREWALL,
        ABILITY_SLUDGE,
        ABILITY_SWORDATTACK,
        ABILITY_SWORDRUSH
    }

    protected override void Start()
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

        chainPull = this.myAbilityInitiator.abilities[AbilityRegister.WRATH_CHAINPULL];
        fireWall = this.myAbilityInitiator.abilities[AbilityRegister.WRATH_FIREWALL];
        sludge = this.myAbilityInitiator.abilities[AbilityRegister.WRATH_SLUDGE];
        swordAttack = this.myAbilityInitiator.abilities[AbilityRegister.WRATH_SWORDATTACK];
        swordRush = this.myAbilityInitiator.abilities[AbilityRegister.WRATH_SWORDRUSH];

        wrath = this.gameObject.GetComponent<WrathP1Actor>();
        currentState = State.WALK;
        currAbility = null;
    }

    public override void DoActorDeath()
    {

    }

    void FixedUpdate()
    {
        if (currAbility != null)
        {
            StartCoroutine(checkIfAbilityDone());
        }

        if (currentState == State.WALK)
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

            case State.ABILITY_CHAINPULL:
                chainPull.Invoke(ref wrath);
                break;

            case State.ABILITY_FIREWALL:
                fireWall.Invoke(ref wrath);
                break;

            case State.ABILITY_SLUDGE:
                sludge.Invoke(ref wrath);
                break;

            case State.ABILITY_SWORDATTACK:
                swordAttack.Invoke(ref wrath);
                break;

            case State.ABILITY_SWORDRUSH:
                swordRush.Invoke(ref wrath);
                break;

            default:
                Debug.LogWarning("WrathP1Actor: Wrath has fallen out of its state machine!");
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

    IEnumerator checkIfAbilityDone()
    {
        // If the currAbility has finished, reset.
        if (currAbility.getIsFinished())
        {
            currAbility = null;
            currentState = State.WALK;

            if(poolType == 'A') // Pool Type A ability has finished. Switch to B
            {
                poolType = 'B';
                canAttack = true;
            }
            else // Pool Type B ability has finished. Switch to A but wait for 3 seconds, then allow wrath to attack. 
            {
                poolType = 'A';
                yield return new WaitForSeconds(3.0f);
                canAttack = true;
            }
        }
    }

    void decideNextState()
    {
        if(canAttack)
        {
            canAttack = false;
            if (poolType == 'A') // Draw an ability from Pool A
            {
                // Determines which ability Wrath will perform

                //int abilityType = (int)Random.Range(0, 3);
                // TESTING CHAINPULL
                int abilityType = 0;

                switch (abilityType)
                {
                    case 0:
                        currentState = State.ABILITY_CHAINPULL;
                        currAbility = chainPull;
                        break;
                    case 1:
                        currentState = State.ABILITY_FIREWALL;
                        currAbility = fireWall;
                        break;
                    case 2:
                        currentState = State.ABILITY_SLUDGE;
                        currAbility = sludge;
                        break;
                    default:
                        Debug.LogWarning("WrathP1Actor: Pool Type A abilityType out of bounds!");
                        break;
                }
            }
            else // Draw an ability from Pool B
            {
                int abilityType = (int)Random.Range(0, 2);
                switch (abilityType)
                {
                    case 0:
                        currentState = State.ABILITY_SWORDATTACK;
                        currAbility = swordAttack;
                        break;
                    case 1:
                        currentState = State.ABILITY_SWORDRUSH;
                        currAbility = swordRush;
                        break;
                    default:
                        Debug.LogWarning("WrathP1Actor: Pool Type B abilityType out of bounds!");
                        break;
                }
            }
        }
        else
        {
            currentState = State.WALK;
        }
    }
}
