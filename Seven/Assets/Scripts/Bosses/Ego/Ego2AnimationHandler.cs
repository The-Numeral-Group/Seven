using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ego2AnimationHandler : ActorAnimationHandler
{
    //METHODS--------------------------------------------------------------------------------------
    //Like the ghost knight, Ego never walks, so it doesn't need an animateWalk

    //Sets the directional parameters for all of Ego2's animations. Will be called every tick of
    //Ego's Actor component.
    public void animateIdle()
    {
        Vector2 faceVec = hostActor.faceAnchor.localPosition;

        this.Flip(faceVec);

        Animator.SetFloat("ego_H", Mathf.Abs(faceVec.x));
        Animator.SetFloat("ego_V", faceVec.y);

    }
}
