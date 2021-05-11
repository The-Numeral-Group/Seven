//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class SimpleStun : ActorEffect
{
    //FIELDS---------------------------------------------------------------------------------------
    //a trigger to try to send to the effectee's animation handler
    private string animTrigger;
    //CONSTRUCTORS---------------------------------------------------------------------------------
    public SimpleStun(string animTrigger)
    {
        this.animTrigger = animTrigger;
    }

    //METHODS--------------------------------------------------------------------------------------
    public bool ApplyEffect(ref Actor actor)
    {
        //play a stun animation, if possible
        actor.myAnimationHandler.TrySetTrigger(animTrigger);

        //lock down the Actor's movement. It's on the applicator to remove this effect,
        //so this will last forever.
        actor.myMovement.LockActorMovement(Mathf.Infinity);

        return true;
    }

    public void RemoveEffect(ref Actor actor)
    {
        //exit the stun animation, if possible
        actor.myAnimationHandler.TrySetTrigger(animTrigger);

        //unlock actor's movement
        actor.myMovement.LockActorMovement(0.0f);
    }
}
