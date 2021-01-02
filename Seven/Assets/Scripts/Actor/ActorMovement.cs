using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class ActorMovement : MonoBehaviour
{
    public float speed;
    public bool movementLocked{ get; private set; }
    private CharacterController2D movementController;
    private Vector2 movementDirection, dragDirection;
    /*This script might also require some extra data for working
    with animations. It'll need to be added later.*/

    protected virtual void Awake()
    {
        movementDirection = dragDirection = Vector2.zero;
        this.movementLocked = false;
    }

    protected virtual void Start()
    {
        movementController = this.gameObject.GetComponent<CharacterController2D>();
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
            //moving
            movementController.Move(dragDirection);
        }
        else
        {
            //calculate a composite movement vector
            Vector2 moveComposite = movementDirection * speed * time.deltaTime;
            moveComposite += dragDirection;

            //actually moving
            movementController.Move(moveComposite);

            //update the direction the actor is facing
            this.gameObject.SendMessage("DoActorUpdateFacing", movementDirection);

            /*Only needed if character can still move. If movement is locked, we assume
            that the drag needs to stop after this movement instance*/
            dragDirection = Vector2.zero;
        }
        
    }

    /*This method is for when the actor wants to move itself*/
    protected virtual void MoveActor(Vector2 direction)
    {
        //the vector is normalized by default to let speed control... well... speed.
        movementDirection = direction.normalized;
    }

    /*This method is for when other things want to move the actor. MoveActor has
    priority, but callers are able to time out MoveActor by specifying a float value
    greater than 0.0f. Using 0.0f or lower will let the actor keep their movement, but
    you'll probably need to keep calling DragActor for extended drags where the player can move.*/
    public virtual void DragActor(Vector2 direction, float actorMoveDisable)
    {
        //this vector isn't normalized because its intensity is independent of the actor's speed
        dragDirection = direction;
        StartCoroutine(LockActorMovement(actorMoveDisable));
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
            this.movementLocked = false;
        }
        else
        {
            this.movementLocked = false;
            yield return null;
        }
        
    }
}
