using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinSlash : ActorAbilityFunction<Actor, int>, PlayerSwordAbility
{
    public override void Invoke(ref Actor user)
    {
        if(usable && isFinished)
        {
            this.user = user;
            isFinished = false;
            InternInvoke(user);
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    protected override int InternInvoke(params Actor[] args)
    {
        //https://docs.unity3d.com/ScriptReference/Animator.Play.html
        //reference for animator methods
        user.myAnimationHandler.Animator.SetTrigger("player_spin");
        user.mySoundManager.PlaySound("PlayerAttack");
        StartCoroutine(CheckAnimationComplete());
        return 0;
    }

    public virtual void FinishSpinSlash()
    {
        isFinished = true;
    }

    IEnumerator CheckAnimationComplete()
    {
        while(!user.myAnimationHandler.Animator.GetCurrentAnimatorStateInfo(0).IsName("SpinAttack"))
        {
            yield return new WaitForFixedUpdate();
        }
        FinishSpinSlash();
    }
}
