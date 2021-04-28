using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PlayerWeaponAbility is more simplified version of WeaponAbility.
// It will handle 3 things:
//    1. If player is ready to attack or not.
//    2. Play Attack animation when player is attacking
//    3. Play Attack audio when player is attacking

public class PlayerWeaponAbility : WeaponAbility, PlayerSwordAbility
{
    // Check if Player Weapon's Hitbox has already hit the enemy.
    // This is used to prevent hitting enemy multiple times with just one attack.
    //public bool hitConnected { get; set; }

    public float lockDuration;

    public override void Invoke(ref Actor user)
    {
        this.user = user;
        // Since player's weapon ability has no cooldown period (like other abilities),
        // this will just check is player has been finished attacking and ready to attack again.
        if (this.usable && this.isFinished)
        {
            this.user = user;
            this.isFinished = false;
            StartCoroutine(coolDown(cooldownPeriod));
            InternInvoke(new Actor[0]);
        }

    }

    protected override int InternInvoke(params Actor[] args)
    {
        StartCoroutine(user.myMovement.LockActorMovement(lockDuration));

        // Play Attack Animation
        PlayerAnimationHandler playerAnimationHandler = user.myAnimationHandler as PlayerAnimationHandler;
        playerAnimationHandler.animateAttack();

        // Play Attack Audio
        user.mySoundManager.PlaySound("PlayerAttack");

        //this.hitConnected = false;
        //invoke the ability as normal
        base.InternInvoke(args);

        return 0;
    }

    // Whenever animation clip has reached to the last frame, it will call this function
    // to let this ability that it has been finished. 
    public void setPlayerWeaponFinished()
    {
        this.isFinished = true;
    }
}
