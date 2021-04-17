using UnityEngine;
using System.Collections;

public class IndulgenceP1Actor : Actor
{
    public int wallSlamTriggerCounter;
    int physicalAttackCounter;
    public int jumpTriggerDistance;
    public Actor target;
    ActorAbility currAbility;
    public State currState;
    bool redirectingPath = false;
    int layerMask;
    IEnumerator MovementCoroutinePtr;
    public enum State
    {
        MOVEMENT,
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
        layerMask =  ~(1 << this.gameObject.layer);
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
        float currDistanceToTarget = Vector2.Distance(target.transform.position, transform.position);
        if (target == null)
        {
            currState = State.WAITING;
        }
        else if (physicalAttackCounter >= wallSlamTriggerCounter)
        {
            physicalAttackCounter = 0;
        }
        else if (currDistanceToTarget > jumpTriggerDistance)
        {
            //do jumpo
        }
        else
        {
            currState = State.MOVEMENT;
        }

        if (currState != State.MOVEMENT)
        {
            this.myMovement.MoveActor(Vector2.zero);
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
            case State.WAITING:
                break;
            default:
                break;
        }
    }

    void MovementPattern()
    {
        if (redirectingPath)
        {
            return;
        }
        //Scan for if player is in path
        Vector2 destination = target.transform.position + (5 * target.faceAnchor.localPosition);
        Vector2 ourPosition = new Vector2(transform.position.x, transform.position.y);
        float distanceToDestination = Vector2.Distance(destination, ourPosition);
        //https://gamedev.stackexchange.com/questions/114121/most-efficient-way-to-convert-vector3-to-vector2
        Vector2 directionToDestination = (destination - ourPosition).normalized;
        //https://answers.unity.com/questions/329389/raycast-ignores-my-layer-mask.html
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToDestination, distanceToDestination, layerMask);
        if (hit.collider != null && hit.collider.tag == target.gameObject.tag)
        {
            Debug.Log(hit.collider.tag);
            //rotate angle of movement slightly scan for a wall.
            float dtheta = Mathf.PI/3f;
            Vector2 newDirection = RotateVector2(directionToDestination, dtheta);
            distanceToDestination = (distanceToDestination/2) / dtheta;
            hit = Physics2D.Raycast(transform.position, newDirection, distanceToDestination);
            //rotate the starting angle the other direction and move that way by default if we
            //collide with a wall
            if (hit.collider != null && hit.collider.tag == "Environment")
            {
                newDirection = RotateVector2(directionToDestination, -dtheta);
            }
            directionToDestination = newDirection;
            StopCoroutine(MovementCoroutinePtr);
            MovementCoroutinePtr = StopRedirecting(1f);
            StartCoroutine(MovementCoroutinePtr);
        }
        this.myMovement.MoveActor(directionToDestination);
        this.myAnimationHandler.animateWalk();
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
