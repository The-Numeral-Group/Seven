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

    [Tooltip("Duration of how long the player will be invincible, must be greater than dodgeDuration")]
    public float invincibleDuration = 1f;

    private float drag = 0.05f;

    /*Similar to ActorAbilityFunction Invoke
    passes an actors movement component to InternalInvoke*/
    public override void Invoke(ref Actor user)
    {
        // Debug.Log("USABLE: " + this.usable);
        // Debug.Log("FINISHED: " + this.isFinished);
        if (this.usable && isFinished)
        {
            this.isFinished = false;
            InternInvoke(user);
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    /*InternInvoke performs a dodge on user's ActorMovement component*/
    protected override int InternInvoke(params Actor[] args)
    {
        args[0].myHealth.SetVulnerable(false, invincibleDuration);
        StartCoroutine(args[0].myMovement.LockActorMovement(dodgeDuration));
        performDodge(args[0]);
        return 0;
    }

    private void performDodge(Actor user)
    {
        // Play audio
        user.mySoundManager.PlaySound("PlayerDodge", 0.8f, 1.2f);

        //PlayerAnimationHandler playerAnimationHandler = user.myAnimationHandler as PlayerAnimationHandler;
        //playerAnimationHandler.animateDodge();

        Vector2 velocity = user.myMovement.movementDirection;

        /*to dodge, we boost forward and lock movement for 1 second. This calculation
        was written by Ram for the prototype*/
        Vector2 dodgeVelocity = velocity + Vector2.Scale(velocity, dodgeDistance *
            new Vector2((Mathf.Log(1f / (Time.deltaTime * drag + 1)) / -Time.deltaTime),
                (Mathf.Log(1f / (Time.deltaTime * drag + 1)) / -Time.deltaTime)));

        user.myMovement.DragActor(dodgeVelocity);


        StartCoroutine(animateDodge(user));
    }

    private IEnumerator animateDodge(Actor user)
    {
        yield return new WaitForSeconds(0.15f);

        PlayerAnimationHandler playerAnimationHandler = user.myAnimationHandler as PlayerAnimationHandler;
        playerAnimationHandler.animateDodge();

        while(playerAnimationHandler.Animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge (No Sword)") || playerAnimationHandler.Animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge (Sword)"))
        {
            yield return new WaitForFixedUpdate();
        }
        this.isFinished = true;
    }

    public void setPlayerDodgeFinished()
    {
        //this.isFinished = true;
    }
}
