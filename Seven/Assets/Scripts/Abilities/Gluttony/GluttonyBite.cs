using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Until we expand on gluttony bit further all it needs to do is 
extend weapon ability and utilize its functions*/
public class GluttonyBite : WeaponAbility
{
    //Calls the base class internal invoke passing in the user actor. Locks the movement of the user.
    protected override int InternInvoke(params Actor[] args)
    {
        StartCoroutine(args[0].myMovement.LockActorMovement(duration));
        base.InternInvoke(args);
        return 0;
    }

}
