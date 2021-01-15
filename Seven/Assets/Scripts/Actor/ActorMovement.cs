using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

/*the rigidbody property hides a similar property of all
gameObjects. However, that property is obsolete so we're
just gonna ignore that warning. This pragma line hides the warning*/
#pragma warning disable CS0108
//this is needed to actually do the moving in-game
[RequireComponent(typeof(SimpleController2D))]
public class ActorMovement : MonoBehaviour
{
    public float speed;

    public bool movementLocked{ get; protected set; }
    public Vector2 movementDirection{ get; protected set; }
    public Vector2 dragDirection{ get; protected set; }

    public Rigidbody2D rigidbody{ get; protected set; }

    private SimpleController2D movementController;
    /*This script might also require some extra data for working
    with animations. It'll need to be added later.*/

    protected virtual void Awake()
    {
        this.movementDirection = this.dragDirection = Vector2.zero;
        this.movementLocked = false;
    }

    protected virtual void Start()
    {
        movementController = this.gameObject.GetComponent<SimpleController2D>();
        this.rigidbody = this.gameObject.GetComponent<Rigidbody2D>();
    }

    /*This method makes the mover take a step every 1/60 of a second
    even if that step goes nowhere and there is no visual step*/
    protected virtual void FixedUpdate()
    {
        InternalMoveActor();
    }


    /*This method is called regularly, and actually makes the character controller
    call to literally move the actor.*/
    protected virtual void InternalMoveActor()
    {
        if(this.movementLocked)
        {
            movementController.Move(this.dragDirection * Time.deltaTime);
        }
        else
        {
            //calculate a composite movement vector
            Vector2 moveComposite = this.movementDirection * speed * Time.deltaTime;
            moveComposite += this.dragDirection;

            //actually moving
            movementController.Move(moveComposite);

            //update the direction the actor is facing
            this.gameObject.SendMessage("DoActorUpdateFacing", this.movementDirection);

            /*Only needed if character can still move. If movement is locked, we assume
            that the drag needs to stop after this movement instance*/
            this.dragDirection = Vector2.zero;
        }
        
    }

    /*This method is for when the actor wants to move itself*/
    public virtual void MoveActor(Vector2 direction)
    {
        //the vector is normalized by default to let speed control... well... speed.
        this.movementDirection = direction.normalized;
    }

    /*This method is for when other things want to move the actor. MoveActor has
    priority, but callers are able to time out MoveActor by specifying a float value
    greater than 0.0f. Using 0.0f or lower will let the actor keep their movement, but
    you'll probably need to keep calling DragActor for extended drags where the player can move.*/
    public virtual void DragActor(Vector2 direction)//, float actorMoveDisable)
    {
        //this vector isn't normalized because its intensity is independent of the actor's speed
        this.dragDirection = direction;
        //StartCoroutine(LockActorMovement(actorMoveDisable));
    }

    /*If something ever needs to lock an Actor's movement, this is the method
    to invoke. Specify a time and movement will stay locked for that long. Passing
    an argument of 0.0f (or less) will unlock movement immediately*/
    public IEnumerator LockActorMovement(float actorMoveDisable)
    {
        if(actorMoveDisable > 0.0f)
        {
            this.movementLocked = true;
            yield return new WaitForSeconds(actorMoveDisable);
            //(Ram) note to self: this is where the line to stop the extra frame of drag application goes.
            this.dragDirection = Vector2.zero;
            this.movementLocked = false;
        }
        else
        {
            this.movementLocked = false;
            yield return null;
        }
        
    }
}
