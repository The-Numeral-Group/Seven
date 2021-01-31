using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Until we expand on gluttony bit further all it needs to do is 
extend weapon ability and utilize its functions*/
public class GluttonyBite : WeaponAbility
{
    //reference to the user
    public Actor user { get; private set; }
    public override void Invoke(ref Actor user, params object[] args)
    {
        //by default, Invoke just does InternInvoke with no arguments
        if(this.usable)
        {
            this.user = user;
            this.isFinished = false;
            StartCoroutine(coolDown(cooldownPeriod));
            InternInvoke(easyArgConvert(args));
        }
        
    }
    
    //Very similar to weaponability internalinvoke, but in this case args[0] is the target.
    protected override int InternInvoke(params Actor[] args)
    {
        if (!args[0])
        {
            return 0;
        }
        StartCoroutine(user.myMovement.LockActorMovement(duration));
        this.hitConnected = false;
        StopCoroutine(sheathe);
        sheathe = SheatheWeapon();
        weaponObject.SetActive(true);
        SpawnWeapon(args[0]);
        StartCoroutine(sheathe);
        return 0;
    }

    //Spawn the weapon in the direction of the target
    protected override void SpawnWeapon(Actor target)
    {
        Vector3 attackDirection = 
            (target.gameObject.transform.position - user.gameObject.transform.position).normalized;
        weaponObject.transform.localPosition = attackDirection * weaponPositionScale;
    }

}
