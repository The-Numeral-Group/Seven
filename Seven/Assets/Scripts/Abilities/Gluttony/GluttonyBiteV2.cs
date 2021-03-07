using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Updated version of gluttony bite which will let the animation handler deal with
creating the hitbox.*/
public class GluttonyBiteV2 : WeaponAbility
{
    protected override void Awake()
    {
        hitConnected = false;
    }
    protected override void Start()
    {
        int i = 0;
        foreach(WeaponHitbox hb in transform.GetComponentsInChildren<WeaponHitbox>())
        {
            Debug.Log("GluttonyBiteV2: Hitbox found");
            if (i < damagePerHitbox.Count)
            {
                hb.damage = damagePerHitbox[i];
            }
            else
            {
                hb.damage = 0;
            }
            i++;
        }
        
    }
    protected override int InternInvoke(params Actor[] args)
    {
        StartCoroutine(user.myMovement.LockActorMovement(duration));
        this.hitConnected = false;
        user.myAnimationHandler.Animator.SetTrigger("Bite");
        return 0;
    }

    public virtual void WeaponIsFinished()
    {
        this.isFinished = true;
    }
}
