using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Until we expand on gluttony bit further all it needs to do is 
extend weapon ability and utilize its functions*/
public class GluttonyBite : WeaponAbility
{
    //Very similar to weaponability internalinvoke, but in this case args[0] is the target.
    protected override int InternInvoke(params Actor[] args)
    {
        if (args.Length == 0)
        {
            return 0;
        }
        StartCoroutine(user.myMovement.LockActorMovement(duration));
        base.InternInvoke(args);
        return 0;
    }

    //Spawn the weapon in the direction of the target
    protected override void SpawnWeapon(params Actor[] args)
    {
        Actor target = args[0];
        Vector3 attackDirection = 
            (target.gameObject.transform.position - user.gameObject.transform.position).normalized;
        weaponObject.transform.localPosition = attackDirection * weaponPositionScale;
    }

}
