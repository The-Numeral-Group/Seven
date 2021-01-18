using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
GhostKnightActor's main function is to run the State Machine that powers
Ghost Knight's boss fight
*/
public class GhostKnightActor : Actor
{

    [Tooltip("Controls how many times should Gluttony use normal attacks before using it's special.")]
    public int specialAttackGate = 7;

    [Tooltip("How close the player should be before Ghost Knight tries to slash them")]
    public float slashRange = 25f;

    [Tooltip("Chance of picking projectile attack compared to slash attack. (1 = 1:1, 2 = 1:2, and so on)")]
    public float projectileRatio = 3f;

    private int specialAttackCounter = 0;

    private Actor ghostKnight;
    private Actor player;
    public State currentState;

    private ActorAbility currAbility;

    public enum State
    {
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

        if(playerObject == null)
        {
            Debug.LogWarning("GhostKnightActor: Ghost Knight can't find the player!");
        }
        else
        {
            player = playerObject.GetComponent<Actor>();
        }

        ghostKnight = this.gameObject.GetComponent<GhostKnightActor>();
        ghostKnight.myHealth.vulnerable = true;
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
            case State.WALK:
                if (currAbility && !currAbility.getUsable())
                {
                    break;
                }
                // check for special attack counter.
                // if it is 7, activate special attack. 
                if (specialAttackCounter >= specialAttackGate)
                {
                    specialAttackCounter = 0;
                    currentState = State.SPECIAL;
                }
                else
                {
                    stepTowardsPlayer();
                    currAbility = null;
                    currentState = decideNextState();
                }
                break;

            case State.PHYSICAL_SLASH:
                var slash = this.myAbilityInitiator.abilities[AbilityRegister.GHOSTKNIGHT_SLASH];

                if(slash.getUsable())
                {
                    currAbility = slash;
                    slash.Invoke(ref ghostKnight);
                }

                currentState = State.WALK;

                break;

            case State.LAUNCH_PROJECTILE:
                var proj = this.myAbilityInitiator.abilities[AbilityRegister.GHOSTKNIGHT_PROJECTILE];

                if (proj.getUsable())
                {
                    currAbility = proj;
                    proj.Invoke(ref ghostKnight);
                }

                currentState = State.WALK;

                break;

            case State.SPECIAL:
                var special = this.myAbilityInitiator.abilities[AbilityRegister.GHOSTKNIGHT_SPECIAL];

                if (special.getUsable())
                {
                    currAbility = special;
                    special.Invoke(ref ghostKnight);
                }

                currentState = State.WALK;

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
    State decideNextState()
    {
        var distanceToPlayer = Vector2.Distance(player.transform.position, this.gameObject.transform.position);
        bool slashReady = this.myAbilityInitiator.abilities[AbilityRegister.GHOSTKNIGHT_SLASH].getUsable();
        bool projectileReady = this.myAbilityInitiator.abilities[AbilityRegister.GHOSTKNIGHT_PROJECTILE].getUsable();

        int whichAtt = (int)Random.Range(1, projectileRatio + 2);
        Debug.Log(whichAtt);

        State nextState;

        if ((whichAtt == 1) && projectileReady)
        {
            nextState = State.LAUNCH_PROJECTILE;
        }
        else if ((whichAtt != 1) && (distanceToPlayer <= slashRange) && slashReady)
        {
            nextState = State.PHYSICAL_SLASH;
        }
        else
        {
            nextState = State.WALK;
        }

        return nextState;
    }
}
