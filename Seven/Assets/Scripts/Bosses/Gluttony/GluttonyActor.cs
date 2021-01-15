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
    public float projectileHealthGate;

    [Tooltip("Controls how many times should Gluttony use normal attacks before using it's special.")]
    public int specialAttackGate = 7;

    private int specialAttackCounter = 0;
    private Actor player;

    public State currentState;
    public enum State
    {
        WALK,
        PHYSICAL_CRUSH,
        AFTER_CRUSH,
        PHYSICAL_BITE,
        AFTER_BITE,
        BEFORE_PHASE0_SPECIAL,
        PHASE0_SPECIAL,
        LAUNCH_PROJECTILE,
        NULL,
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        //player = 
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
                    currentState = State.BEFORE_PHASE0_SPECIAL;
                }
                else
                {
                    CheckDistance();
                    StartCoroutine(WalkToTarget());
                }
                break;

            case State.PHYSICAL_CRUSH:
                StartCoroutine(MoveToTarget());
                break;

            case State.AFTER_CRUSH:
                break;

            case State.PHYSICAL_BITE:
                StartCoroutine(PhysicalBite());
                break;

            case State.AFTER_BITE:
                break;

            case State.BEFORE_PHASE0_SPECIAL:
                StartCoroutine(PhaseOne_SpecialA());
                break;

            case State.PHASE0_SPECIAL:
                StartCoroutine(PhaseOne_SpecialA_Activated());
                break;

            case State.LAUNCH_PROJECTILE:
                StartCoroutine(GenerateProjectiles());
                break;

            case State.NULL:
                break;

            default:
                break;
        }
    }

    //GetMovementDirection isn't needed, an equivilant exists in ActorMovement

    bool ShouldProjectile()
    {
        //get the projectile ability, if it exists
        var projectiles = this.myAbilityInitiator.abilityDict?[AbilityRegister.GLUTTONY_PROJECTILE];

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
        //this.my
    }
}
