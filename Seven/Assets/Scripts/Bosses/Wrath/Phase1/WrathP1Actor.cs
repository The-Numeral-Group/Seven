using System.Collections;
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

    public enum State
    {
        WAITING,
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
        // THINGS THAT NEED TO BE CLARIFIED BY DESIGN:
        // 1. Design Document says that Wrath will perform an ability from Pool B immediately after ability from Pool A has finished,
        //    will there be any delay after Pool B ability has finished? (Any delay after Pool B ability?)
        //    If yes, how long?
        // 2. What if Wrath chose Chain Pull form Pool A but that skill is on cooldown?
        //    Are there going to be cooldowns for skills?
    }
}
