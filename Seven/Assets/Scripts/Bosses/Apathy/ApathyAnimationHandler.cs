using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApathyAnimationHandler : ActorAnimationHandler
{
    //METHODS--------------------------------------------------------------------------------------
    //Like the ghost knight, Ego never walks, so it doesn't need an animateWalk

    //Sets the directional parameters for all of Apathy's animations. Will be called every tick of
    //Apathy's Actor component.
    public void animateIdle()
    {
        Vector2 faceVec = hostActor.faceAnchor.localPosition;

        this.Flip(faceVec);

        Animator.SetFloat("apathy_H", Mathf.Abs(faceVec.x));
        Animator.SetFloat("apathy_V", faceVec.y);

    }

    public void animateThrow()
    {
        this.Animator.SetTrigger("apathy_throw");
    }

    public void animateSwat()
    {
        this.Animator.SetTrigger("apathy_swat");
    }
}