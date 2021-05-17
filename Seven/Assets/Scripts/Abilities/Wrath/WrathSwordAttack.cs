using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathSwordAttack : ActorAbilityFunction<Actor, int>
{
    public float duration;

    private WrathAnimationHandler wrathAnimationHandler;

    private Actor wrath;

    public override void Invoke(ref Actor user)
    {
        if (usable)
        {
            isFinished = false;
            InternInvoke(user);
        }
    }
    protected override int InternInvoke(params Actor[] args)
    {
        // Making sure the movementDirection and dragDirection have been resetted.
        args[0].myMovement.MoveActor(Vector2.zero);
        args[0].myMovement.DragActor(Vector2.zero);

        wrath = args[0];
        wrathAnimationHandler = wrath.myAnimationHandler as WrathAnimationHandler;

        StartCoroutine(wrath.myMovement.LockActorMovement(this.duration));
        PerformSwordAttack();
        StartCoroutine(SwordAttackFinished());
        return 0;
    }
    private void PerformSwordAttack()
    {
        wrathAnimationHandler.animateSwordAttack();
    }

    private IEnumerator SwordAttackFinished()
    {
        yield return new WaitForSeconds(this.duration);
        // Resetting the dragDirection
        wrath.myMovement.DragActor(Vector2.zero);
        isFinished = true;
    }
}
