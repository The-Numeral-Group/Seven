using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GluttonyBite : WeaponAbility
{
    //Until we expand on gluttony bit further all it needs to do is extend weapon ability and utilize its functions.
    protected override int InternInvoke(params Actor[] args)
    {
        StartCoroutine(args[0].myMovement.LockActorMovement(duration));
        base.InternInvoke(args);
        return 0;
    }

}
