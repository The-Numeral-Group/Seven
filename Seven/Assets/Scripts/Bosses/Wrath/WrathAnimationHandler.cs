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

    public void animateSwordRush(Vector2 facingDirection)
    {
        // check facingDirection
        if(facingDirection.y >= 0.0f)
        {
            Animator.SetBool("Wrath_SwordRush_N", true);
            Animator.SetBool("Wrath_SwordRush_S", false);
        }
        else
        {
            Animator.SetBool("Wrath_SwordRush_S", true);
            Animator.SetBool("Wrath_SwordRush_N", false);
        }
    }
}
