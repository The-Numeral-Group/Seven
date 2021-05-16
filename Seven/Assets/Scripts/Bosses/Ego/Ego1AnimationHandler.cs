using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ego1AnimationHandler : ActorAnimationHandler
{
    //Runs the logic for switching Ego1's walking animations
    public override void animateWalk()
    {
        Vector2 faceVec = hostActor.faceAnchor.localPosition;

        this.Flip(faceVec);

        Animator.SetFloat("ego_H", Mathf.Abs(faceVec.x));
        Animator.SetFloat("ego_V", faceVec.y);

        /*If direction is 0, then Ego1 isn't moving. That information is passed to the animator, 
        which will then swap between Ego's idle and walk automatically. We don't bother with a
        TrySet because Ego1 should have this property in its unique animator.*/
        this.Animator.SetBool(
            "ego_walking", hostActor.myMovement.movementDirection != Vector2.zero);
    }

    //picks a random flex and animates it
    public System.Func<bool> animateFLEX()
    {
        //Step 1: pick a random flex to do by generating a 0 to 1 float
        this.Animator.SetFloat("ego_flextype", Random.value);

        //Step 2: actually... you know... animate the flex
        //the trigger will always be there, we want the bool
        return this.TryFlaggedSetTrigger("ego_flex");
    }

    //Toggles Ego between its swagger and sprint animations
    public void SetSwagger(bool swagger)
    {
        if(swagger)
        {
            this.Animator.SetFloat("ego_swagger", 1f);
        }
        else
        {
            this.Animator.SetFloat("ego_swagger", 0f);
        }
        
    }
}
