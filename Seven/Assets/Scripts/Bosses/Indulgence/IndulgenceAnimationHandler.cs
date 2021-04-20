using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class IndulgenceAnimationHandler : ActorAnimationHandler
{
    SpriteRenderer sp;

    //reference sometimes not setup in start before game starts running.
    void Awake()
    {
        hostActor = GetComponent<Actor>();
    }

    public override void Start()
    {
        base.Start();
        sp = GetComponent<SpriteRenderer>();
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

    public void Flip(Vector3 direction)
    {
        if (direction.x < 0) //left
        {
            sp.flipX = true;
        }
        else if (direction.x > 0) //right
        {
            sp.flipX = false;
        }
    }
}
