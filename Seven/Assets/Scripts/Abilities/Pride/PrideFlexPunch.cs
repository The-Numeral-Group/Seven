using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*I (Thomas) basically copied the implemnetation of Gluttony's bite.*/
public class PrideFlexPunch : WeaponAbility
{
    //Calls the base class internal invoke passing in the user actor. Locks the movement of the user.
    protected override int InternInvoke(params Actor[] args)
    {
        StartCoroutine(args[0].myMovement.LockActorMovement(duration));
        base.InternInvoke(args);
        return 0;
    }
}
