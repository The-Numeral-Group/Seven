using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class IndulgenceAnimationHandler : ActorAnimationHandler
{

    //reference sometimes not setup in start before game starts running.
    void Awake()
    {
        hostActor = GetComponent<Actor>();
    }
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
}
