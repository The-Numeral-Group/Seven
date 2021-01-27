using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*I (Thomas) am genericing the return type to int because 
I don't care about the return
(Ram) the dodge equation needs to change in order to work peoperly with how we now handle movement*/
public class Dodge : ActorAbilityFunction<Actor, int>
{
    //how far the actor goes when they doddge
    [Tooltip("How far the actor will dodge.")]
    public float dodgeDistance = 15.0f;

    //how long before the actor can move after dodging
    [Tooltip("How long it will for the actor to be able to move after dodging.")]
    public float movementLockForDodge = 1.0f;

    /*Similar to ActorAbilityFunction Invoke
    passes an actors movement component to InternalInvoke*/
    public override void Invoke(ref Actor user)
    {
        if(this.usable && isFinished)
        {
            this.isFinished = false;
            StartCoroutine(coolDown(cooldownPeriod));
            InternInvoke(user);
        }
    }

    /*InternInvoke performs a dodge on user's ActorMovement component*/
    protected override int InternInvoke(params Actor[] args)
    {
        args[0].myHealth.vulnerable = false;
        //we assume that the needed ActorMovement is the first thing
        //in args, that's what the 0 is for.

        Vector2 velocity = args[0].myMovement.movementDirection;
        float drag = args[0].myMovement.rigidbody.drag;

        /*to dodge, we boost forward and lock movement for 1 second. This calculation
        was written by Ram for the prototype*/
        Vector2 dodgeVelocity = velocity + Vector2.Scale(velocity, dodgeDistance * 
            new Vector2((Mathf.Log(1f/ (Time.deltaTime * drag + 1))/-Time.deltaTime),
                (Mathf.Log(1f/ (Time.deltaTime * drag + 1))/-Time.deltaTime)));
        StartCoroutine(args[0].myMovement.LockActorMovement(movementLockForDodge));
        StartCoroutine(MakeVulnerable(args[0]));
        args[0].myMovement.DragActor(dodgeVelocity);
        return 0;
    }

    IEnumerator MakeVulnerable(Actor user)
    {
        yield return new WaitForSeconds(movementLockForDodge);
        isFinished = true;
        user.myHealth.vulnerable = true;

    }
}
