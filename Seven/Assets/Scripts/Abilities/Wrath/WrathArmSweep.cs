using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Wraths sweep ability. Animation handles enabling and disabling of hitbox
public class WrathArmSweep : WeaponAbility
{
    public GameObject alreadyInstantiatedWeaponObject;
    protected override void Start()
    {
        weaponObject = alreadyInstantiatedWeaponObject;
        weaponObject.transform.parent = this.gameObject.transform;
        weaponObject.SetActive(false);

        //set damage of hitboxes
        if (damagePerHitbox.Count > 0)
        { 
            SetDamage(damagePerHitbox[0]);
        }
        else
        {
            SetDamage(0);
        }
    }

    protected override int InternInvoke(params Actor[] args)
    {
        this.hitConnected = false;
        if(animTrigger.Length != 0)
        {
            user.myAnimationHandler.TrySetTrigger(animTrigger);
        }
        StartCoroutine(CheckIfAnimFinished());
        return 0;
    }

    IEnumerator CheckIfAnimFinished()
    {
        while(this.user.myAnimationHandler.IsInState(animTrigger))
        {
            yield return new WaitForFixedUpdate();
        }
        FinishArmSweep();
    }

    protected virtual void FinishArmSweep()
    {
        weaponObject.SetActive(false);
        this.isFinished = true;
    }
}
