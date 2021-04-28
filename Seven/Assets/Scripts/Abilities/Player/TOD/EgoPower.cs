using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EgoPower : EgoLaser
{
    //FIELDS---------------------------------------------------------------------------------------
    [Header("Power Settings")]
    [Tooltip("How long the user should be locked in a charge state before the laser" + 
        " is shown.")]
    public float chargeTime = 2f;

    [Tooltip("How many times this ability can be used by its user per scene.")]
    public int maxUses = 2;

    //internal counter of uses
    private int useCount;
    
    //METHODS--------------------------------------------------------------------------------------
    //wrapper for a coroutine that handles the duration of the actual laser
    protected override int InternInvoke(params Vector3[] args)
    {
        //only execute the ability if there are enough uses remaining
        if(useCount < maxUses)
        {
            usable = false;
            StartCoroutine(ChargeInvokation(args[0]));
            ++useCount;
            return 0;
        }

        return 1;
    }

    IEnumerator ChargeInvokation(params Vector3[] args)
    {
        //lock the user in place for the charge duration
        yield return user.myMovement.LockActorMovementOnly(chargeTime);

        /*then let EgoLaser handle the rest
        we default the argument to the user's faceAnchor, because the laser
        should be fired in the direction the user is facing when the charge
        is complete*/
        base.InternInvoke(user.faceAnchor.position);
    }
}
