using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//we get the character controller from ActorMovement
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : ActorMovement
{
    // If player can move or not. 
    public bool canMove = true;

    void OnMovement(InputValue input)
    {
        if(canMove)
        {
            //Debug.Log(input);
            base.MoveActor(input.Get<Vector2>());
            if (input.Get<Vector2>() == Vector2.zero)
            {
                hostActor.mySoundManager.StopSound("PlayerRun");
            }
            else
            {
                hostActor.mySoundManager.PlaySound("PlayerRun");
            }
        }
    }

    public override void AnimateWalkActor()
    {
        if(canMove)
        {
            hostActor.myAnimationHandler.animateWalk();
        }
    }
}
