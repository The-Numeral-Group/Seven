using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WrathAnimationHandler : ActorAnimationHandler
{
    void Awake()
    {
        hostActor = GetComponent<Actor>();
    }

    public override void animateWalk()
    {
        var movementDir = hostActor.myMovement.movementDirection;
        Animator.SetFloat("Wrath_H", movementDir.x);
        Animator.SetFloat("Wrath_V", movementDir.y);
    }

    public void resetMovementDirection()
    {
        Animator.SetFloat("Wrath_H", 0.0f);
        Animator.SetFloat("Wrath_V", -1.0f);
    }

    public void animateSwordAttack()
    {
        Animator.SetTrigger("Wrath_SwordAttack");
    }

    public void animateSwordRush()
    {
        Animator.SetTrigger("Wrath_SwordRush");
    }
}
