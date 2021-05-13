using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathSwordAttack : ActorAbilityFunction<Actor, int>
{
    public float duration;

    public override void Invoke(ref Actor user)
    {
        if (usable)
        {
            isFinished = false;
            InternInvoke(user);
            //StartCoroutine(coolDown(cooldownPeriod));
        }
    }
    protected override int InternInvoke(params Actor[] args)
    {
        // Making sure the movementDirection and dragDirection have been resetted.
        args[0].myMovement.MoveActor(Vector2.zero);
        args[0].myMovement.DragActor(Vector2.zero);

        StartCoroutine(args[0].myMovement.LockActorMovement(this.duration));
        StartCoroutine(SwordAttackFinished(args[0]));
        return 0;
    }

    private IEnumerator SwordAttackFinished(Actor user)
    {
        yield return new WaitForSeconds(this.duration);
        // Resetting the dragDirection
        user.myMovement.DragActor(Vector2.zero);
        isFinished = true;
    }
}
