using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostKnightSlash : ActorAbilityFunction<Actor, int>
{
    //How long this entire process should take.
    public float duration = 2f;
    //Time when ghost knight moves forward
    public float moveForward = 1f;

    GhostKnightAnimationHandler ghostKnightAnimationHandler;

    public bool slashPlaySound = false;

    public override void Invoke(ref Actor user)
    {
        if (usable)
        {
            isFinished = false;
            this.user = user;
            InternInvoke(user);
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    protected override int InternInvoke(params Actor[] args)
    {
        if (this.duration <= 0f)
        {
            Debug.Log("GhostKnightPhaseChange: duration must be greater than 0");
            this.duration = 2f;
        }
        StartCoroutine(args[0].myMovement.LockActorMovement(duration));

        ghostKnightAnimationHandler = args[0].myAnimationHandler as GhostKnightAnimationHandler;

        int whichAtt = (int)Random.Range(1, 3);
        if (whichAtt == 1)
        {
            PerformVSlash(args[0]);
        }
        else
        {
            PerformHSlash(args[0]);
        }
        return 0;
    }

    public void Update()
    {
        if(slashPlaySound)
        {
            slashPlaySound = false;
            user.mySoundManager.PlaySound("PhysicalSwing");
        }
    }

    private void PerformVSlash(Actor user)
    {
        ghostKnightAnimationHandler.animateVSlash();
        user.myMovement.DragActor(new Vector2(0.0f, -0.5f));

        StartCoroutine(SlashFinished(user));
    }
    private void PerformHSlash(Actor user)
    {
        ghostKnightAnimationHandler.animateHSlash();
        user.myMovement.DragActor(new Vector2(0.0f, -0.5f));

        StartCoroutine(SlashFinished(user));
    }
    private IEnumerator SlashFinished(Actor user)
    {
        yield return new WaitForSeconds(this.duration);
        user.myMovement.DragActor(new Vector2(0.0f, 0.0f));
        isFinished = true;
    }
}
