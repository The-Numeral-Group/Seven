using UnityEngine;
using System.Collections;

public class IndulgenceP1Actor : Actor
{
    public int wallSlamTriggerCounter;
    int physicalAttackCounter;
    public float jumpTriggerDistance;
    public float physicalTriggerDistance;
    public Actor target;
    Actor self;
    ActorAbility currAbility;
    public State currState;
    public Collider2D wallCollider;
    bool redirectingPath = false;
    int layerMask;
    IEnumerator MovementCoroutinePtr;
    public enum State
    {
        MOVEMENT,
        PHYSICAL,
        WALLCRAWL,
        WAITING,
    }

    protected virtual void Awake()
    {
        currAbility = null;
        physicalAttackCounter = 0;
        MovementCoroutinePtr = StopRedirecting(1f);
    }

    protected override void Start()
    {
        base.Start();
        self = this as Actor;
        layerMask =  ~(1 << this.gameObject.layer);
        SetupTarget();
    }

    public void SetupTarget()
    {
        if (target == null)
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.GetComponent<Actor>();
                Collider2D tCollider = target.GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(wallCollider, tCollider);
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
        float currDistanceToTarget = Vector2.Distance(target.transform.position, transform.position);
        currAbility = null;
        if (target == null)
        {
            currState = State.WAITING;
        }
        else if (physicalAttackCounter >= wallSlamTriggerCounter && this.myAbilityInitiator.abilities[AbilityRegister.INDULGENCE_WALLCRAWL].getUsable())
        {
            physicalAttackCounter = 0;
            currState = State.WALLCRAWL;
        }
        else if (currDistanceToTarget > jumpTriggerDistance)
        {
            //do jumpo
        }
        else if (currDistanceToTarget < physicalTriggerDistance && this.myAbilityInitiator.abilities[AbilityRegister.INDULGENCE_PHYSICAL].getUsable())
        {
            physicalAttackCounter += 1;
            currState = State.PHYSICAL;
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
                currAbility = this.myAbilityInitiator.abilities[AbilityRegister.INDULGENCE_PHYSICAL];
                Vector2 direction = target.transform.position - this.transform.position;
                currAbility.Invoke(ref self, direction);
                break;
            case State.WALLCRAWL:
                currAbility = this.myAbilityInitiator.abilities[AbilityRegister.INDULGENCE_WALLCRAWL];
                currAbility.Invoke(ref self, target);
                break;
            case State.WAITING:
                break;
            default:
                break;
        }
    }

    void MovementPattern()
    {
        
        //Scan for if player is in path
        Vector2 destination = target.transform.position + (5 * target.faceAnchor.localPosition);
        Vector2 ourPosition = new Vector2(transform.position.x, transform.position.y);
        float distanceToDestination = Vector2.Distance(destination, ourPosition);
        if (distanceToDestination > jumpTriggerDistance && this.myAbilityInitiator.abilities[AbilityRegister.INDULGENCE_CRUSH].getUsable())
        {
            currAbility = this.myAbilityInitiator.abilities[AbilityRegister.INDULGENCE_CRUSH];
            IndulgenceCrush crush = currAbility as IndulgenceCrush;
            crush.useTrackingCrush = false;
            crush.overrideCooldown = false;
            crush.SetTotalAbilityDuration(1f, 1f, 0.25f, 0.25f, 0.5f);
            currAbility.Invoke(ref self, target);
        }
        else if (redirectingPath)
        {
            return;
        }
        else //Actual movement
        {
            //https://gamedev.stackexchange.com/questions/114121/most-efficient-way-to-convert-vector3-to-vector2
            Vector2 directionToDestination = (destination - ourPosition).normalized;
            //https://answers.unity.com/questions/329389/raycast-ignores-my-layer-mask.html
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToDestination, distanceToDestination, layerMask);
            if (hit.collider != null && hit.collider.tag == target.gameObject.tag)
            {
                RedirectPath(ref directionToDestination, distanceToDestination);
            }
            this.myMovement.MoveActor(directionToDestination);
            this.myAnimationHandler.animateWalk();
        }
    }

    void RedirectPath(ref Vector2 directionToDestination, float distanceToDestination)
    {
        //rotate angle of movement slightly scan for a wall.
        float dtheta = Mathf.PI/3f;
        Vector2 newDirection = RotateVector2(directionToDestination, dtheta);
        distanceToDestination = (distanceToDestination/2) / dtheta;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, newDirection, distanceToDestination);
        //rotate the starting angle the other direction and move that way by default if we
        //collide with a wall
        if (hit.collider != null && hit.collider.tag == "Environment")
        {
            newDirection = RotateVector2(directionToDestination, -dtheta);
        }
        directionToDestination = newDirection;
        StopCoroutine(MovementCoroutinePtr);
        MovementCoroutinePtr = StopRedirecting(1.5f);
        StartCoroutine(MovementCoroutinePtr);
    }

    //https://forum.unity.com/threads/whats-the-best-way-to-rotate-a-vector2-in-unity.729605/
    Vector2 RotateVector2(Vector2 v, float dtheta)
    {
        return new Vector2(v.x * Mathf.Cos(dtheta) - v.y * Mathf.Sin(dtheta),
                            v.x * Mathf.Sin(dtheta) + v.y * Mathf.Cos(dtheta));
    }

    IEnumerator StopRedirecting(float seconds)
    {
        redirectingPath = true;
        yield return new WaitForSeconds(seconds);
        redirectingPath = false;
    }
}
