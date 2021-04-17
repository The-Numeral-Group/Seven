using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndulgenceAnimationHandler : ActorAnimationHandler
{
    public override void animateWalk()
    {
        if (hostActor.myMovement.isMoving())
        {
            Animator.SetBool("walking", true);
            Animator.SetFloat("movement_V", hostActor.myMovement.movementDirection.y);
        }
        else
        {
            Animator.SetBool("walking", false);
        }
    }
}
