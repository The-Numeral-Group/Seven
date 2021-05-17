using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndulgenceP2Actor : Actor
{
    public int specialAttackTrigger = 7;
    int attackCounter;
    public State currState;
    ActorAbility currAbility;
    public Actor target;
    Vector2[] movementDirections;
    Actor self;
    public GameObject gameSaveManager;
    public enum State
    {
        MOVEMENT,
        PHYSICAL,
        PROJECTILE,
        SPECIAL,
        WAITING,
    }

    void Awake()
    {
        attackCounter = 0;
        movementDirections = new Vector2[8];
        float dtheta = Mathf.PI/4;
        for(int i = 0; i < 8; i++)
        {
            Vector2 direction = new Vector2(Mathf.Cos(i * dtheta), Mathf.Sin(i * dtheta));
            movementDirections[i] = direction;
        }
        currAbility = null;
        self = this as Actor;
    }

    protected override void Start()
    {
        base.Start();
        SetupTarget();
    }

    public override void DoActorDeath()
    {
        // Save Positions
        var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];
        gameSaveManager.GetComponent<GameSaveManager>().setBoolValue(true, 18);
        this.gameSaveManager.GetComponent<GameSaveManager>().setVectorValue(playerObject.transform.position, 2);
        this.gameSaveManager.GetComponent<GameSaveManager>().setVectorValue(transform.position, 6);

        // Update Flag for game save

        // Load Death cutscene 
        MenuManager.PAUSE_MENU.LoadScene("Indulgence_BossD");
    }

    public void SetupTarget()
    {
        if (target == null)
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.GetComponent<Actor>();
            }
            else
            {
                Debug.LogWarning("IndulgenceP1Actor: unable to find target for indulgence.");
            } 
        }
    }

    void FixedUpdate()
    {
        if (currAbility == null || currAbility.getIsFinished())
        {
            EvaluateState();
        }
    }

    protected virtual void EvaluateState()
    {
        if (target == null)
        {
            currState = State.WAITING;
        }
        else if(this.myAbilityInitiator.abilities[AbilityRegister.INDULGENCE_SPECIAL].getUsable() && 
            attackCounter == specialAttackTrigger)
        {
            attackCounter = 0;
            currState = State.SPECIAL;
        }
        else if (this.myAbilityInitiator.abilities[AbilityRegister.INDULGENCE_CHARGE].getUsable())
        {
            attackCounter++;
            currState = State.PHYSICAL;
        }
        else if (this.myAbilityInitiator.abilities[AbilityRegister.INDULGENCE_PROJECTILE].getUsable())
        {
            attackCounter++;
            currState = State.PROJECTILE;
        }
        else
        {
            currState = State.MOVEMENT;
        }

        if (currState != State.MOVEMENT)
        {
            this.myMovement.MoveActor(Vector2.zero);
            this.myAnimationHandler.animateWalk();
        }
        ExecuteState(currState);
    }

    protected virtual void ExecuteState(State state)
    {
        switch(state)
        {
            case State.MOVEMENT:
                MovementPattern();
                break;
            case State.PHYSICAL:
                currAbility = this.myAbilityInitiator.abilities[AbilityRegister.INDULGENCE_CHARGE];
                currAbility.Invoke(ref self, target);
                break;
            case State.PROJECTILE:
                this.myAnimationHandler.Animator.SetTrigger("projectile_attack");
                Vector2 direction = target.transform.position - this.transform.position;
                currAbility = this.myAbilityInitiator.abilities[AbilityRegister.INDULGENCE_PROJECTILE];
                currAbility.Invoke(ref self, direction);
                break;
            case State.SPECIAL:
                currAbility = this.myAbilityInitiator.abilities[AbilityRegister.INDULGENCE_SPECIAL];
                currAbility.Invoke(ref self, target);
                break;
            case State.WAITING:
                break;
            default:
                break;
        }
    }

    protected virtual void MovementPattern()
    {
        Vector2 directionToTarget = (target.transform.position - this.transform.position).normalized;
        //The following is incredibly inefficient.
        //When polishing better to change this array into a tree structure for comparisons.
        float smallestAngle = Mathf.PI;
        Vector2 movementDirection = Vector2.right;
        foreach(Vector2 direction in movementDirections)
        {
            float currAngle = Mathf.Acos(Vector2.Dot(directionToTarget, direction) / (directionToTarget.magnitude * direction.magnitude));
            if (currAngle <= smallestAngle)
            {
                movementDirection = direction;
                smallestAngle = currAngle;
            }
        }
        this.myMovement.MoveActor(movementDirection);
        this.myAnimationHandler.animateWalk();
    }
}
