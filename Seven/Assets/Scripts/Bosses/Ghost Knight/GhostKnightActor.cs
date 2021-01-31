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

    [Tooltip("Ghost Knight's physical contact damage")]
    public float damage = 1f;

    [Tooltip("Delay before Ghost Knight starts attacking")]
    public float introDelay = 2f;
    private bool attackEnabled = false;

    private int specialAttackCounter = 1;

    private Actor ghostKnight;
    private Actor player;

    public State currentState;
    private ActorAbility currAbility;

    private ActorAbility slash;
    private ActorAbility proj;
    private ActorAbility special;

    private PointEffector2D pointEffector;

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

        pointEffector = this.gameObject.GetComponent<PointEffector2D>();

        ghostKnight = this.gameObject.GetComponent<GhostKnightActor>();
        ghostKnight.myHealth.vulnerable = true;
        currentState = State.WALK;
        currAbility = null;

        StartCoroutine(introDelayStart());
    }

    // When the game starts, the ghost knight will try to cast any attack with movementDirection
    // of vector zero. This will cause some issue with knockback, casting abilities that require position.
    // To avoid this, I have added a short delay before the ghost knight attacks. 
    // When the game starts, the ghost knight will walk for short time then try to attack. 
    private IEnumerator introDelayStart()
    {
        EvaluateState(currentState);
        yield return new WaitForSeconds(this.introDelay);
        attackEnabled = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (attackEnabled)
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
    // Handles the physical contact with the player effect.
    // Knockback Effect is handled with Point Effector 2D Component.
    void OnTriggerEnter2D(Collider2D collider)
    {
        // Only collide with player
        if (collider.gameObject.tag == "Player")
        {
            pointEffector.enabled = true;

            var playerHealth = collider.gameObject.GetComponent<ActorHealth>();

            //or a weakpoint if there's no regular health
            if (playerHealth == null) { collider.gameObject.GetComponent<ActorWeakPoint>(); }

            //if the enemy can take damage (if it has an ActorHealth component),
            //hurt them. Do nothing if they can't take damage.
            if (playerHealth != null)
            {
                if (!playerHealth.vulnerable)
                {
                    return;
                }
                playerHealth.takeDamage(damage);
            }
        }
        else
        {
            //Debug.Log(collider);
            pointEffector.enabled = false;
        }
    }
}