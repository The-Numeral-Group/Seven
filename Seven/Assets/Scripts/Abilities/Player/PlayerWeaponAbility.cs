using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PlayerWeaponAbility is more simplified version of WeaponAbility.
// It will handle 3 things:
//    1. If player is ready to attack or not.
//    2. Play Attack animation when player is attacking
//    3. Play Attack audio when player is attacking
public class PlayerWeaponAbility : ActorAbilityFunction<Actor, int>
{

    public override void Invoke(ref Actor user)
    {
        this.user = user;
        // Since player's weapon ability has no cooldown period (like other abilities),
        // this will just check is player has been finished attacking and ready to attack again.
        if (this.isFinished)
        {
            this.isFinished = false;
            InternInvoke(new Actor[0]);
        }

    }

    protected override int InternInvoke(params Actor[] args)
    {
        // Play Attack Animation
        PlayerAnimationHandler playerAnimationHandler = user.myAnimationHandler as PlayerAnimationHandler;
        playerAnimationHandler.animateAttack();

        // Play Attack Audio
        user.mySoundManager.PlaySound("PlayerAttack");
        return 0;
    }

    // Whenever animation clip has reached to the last frame, it will call this function
    // to let this ability that it has been finished. 
    public void setPlayerWeaponFinished()
    {
        this.isFinished = true;
    }
}
