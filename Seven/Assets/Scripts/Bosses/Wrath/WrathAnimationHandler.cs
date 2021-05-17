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

    public void animateSwordAttack()
    {
        Animator.SetTrigger("Wrath_SwordAttack");
    }
}
