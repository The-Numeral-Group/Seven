using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostKnightSlash : ActorAbilityFunction<Actor, int>
{
    //How long this entire process should take.
    public float duration = 2f;
    //How long the attack animation lasts;
    public float animationDuration = 1f;

    GhostKnightAnimationHandler ghostKnightAnimationHandler;

    public override void Invoke(ref Actor user)
    {
        if (usable)
        {
            isFinished = false;
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
    
    private void PerformVSlash(Actor user)
    {
        ghostKnightAnimationHandler.animateVSlash();
        StartCoroutine(SlashFinished());
    }
    private void PerformHSlash(Actor user)
    {
        ghostKnightAnimationHandler.animateHSlash();
        StartCoroutine(SlashFinished());
    }
    private IEnumerator SlashFinished()
    {
        yield return new WaitForSeconds(this.duration);
        isFinished = true;
    }
}
