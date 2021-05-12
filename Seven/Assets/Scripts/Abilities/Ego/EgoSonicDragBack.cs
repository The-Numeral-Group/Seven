using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EgoSonicDragBack : DragBackWeaponHitbox
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("How long a stun from this attack should last.")]
    public float stunDuration = 2.75f;

    [Tooltip("A trigger to start the target's stun animation, if any. Will not error if invalid")]
    public string stunAnimTrigger = "";

    //METHODS--------------------------------------------------------------------------------------
    /*Standard DragBack, but attempts to stun the target as well*/
    protected override IEnumerator DragBack(ActorMovement mover)
    {
        Debug.Log("trying for components on " + mover.gameObject.name);
        Actor moverActor;
        if(mover.gameObject.TryGetComponent(out moverActor))
        {
            Debug.LogWarning("EgoSonicDragBack: target lacks an Actor, so stun could not be" + 
                " safely applied.");
        }
        else
        {
            moverActor.myEffectHandler.AddTimedEffect(
                new SimpleStun(stunAnimTrigger), 
                stunDuration
            );
        }

        mover.gameObject.GetComponent<ActorEffectHandler>()?.AddTimedEffect(
            new SimpleStun(stunAnimTrigger), 
            stunDuration
        );

        yield return base.DragBack(mover);
    }
}
