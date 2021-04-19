using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndulgenceAnimationHandler : ActorAnimationHandler
{
    bool facingRight = true;
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
        //Flip(hostActor.myMovement.movementDirection);
    }

    public void Flip(Vector3 direction)
    {
        Vector3 flip = transform.localScale;
        if (direction.x < 0 && facingRight)
        {
            flip.x *= -1;
            facingRight = false;
        }
        else if (direction.x > 0 && !facingRight)
        {
            flip.x *= -1;
            facingRight = true;
        }
        transform.localScale = flip;
    }
}
