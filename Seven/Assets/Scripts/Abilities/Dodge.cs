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
    public float dodgeDistance;

    [Tooltip("Duration of the dodge process")]
    public float dodgeDuration;

    private float drag = 0.05f;

    /*Similar to ActorAbilityFunction Invoke
    passes an actors movement component to InternalInvoke*/
    public override void Invoke(ref Actor user)
    {
        if(this.usable && isFinished)
        {
            this.isFinished = false;
            InternInvoke(user);
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    /*InternInvoke performs a dodge on user's ActorMovement component*/
    protected override int InternInvoke(params Actor[] args)
    {
        args[0].myHealth.vulnerable = false;
        StartCoroutine(args[0].myMovement.LockActorMovement(dodgeDuration));
        StartCoroutine(MakeVulnerable(args[0]));
        StartCoroutine(performDodge(args[0]));
        return 0;
    }

    private IEnumerator performDodge(Actor user)
    {

        Vector2 velocity = user.myMovement.movementDirection;

        /*to dodge, we boost forward and lock movement for 1 second. This calculation
        was written by Ram for the prototype*/
        Vector2 dodgeVelocity = velocity + Vector2.Scale(velocity, dodgeDistance *
            new Vector2((Mathf.Log(1f / (Time.deltaTime * drag + 1)) / -Time.deltaTime),
                (Mathf.Log(1f / (Time.deltaTime * drag + 1)) / -Time.deltaTime)));

        user.myMovement.DragActor(dodgeVelocity);
        yield return new WaitForSeconds(dodgeDuration);
        isFinished = true;
    }

    IEnumerator MakeVulnerable(Actor user)
    {
        yield return new WaitForSeconds(dodgeDuration);
        user.myHealth.vulnerable = true;

    }
}
