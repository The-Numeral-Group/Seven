using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apathetic : ActorAbilityFunction<int, int>, ActorEffect
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("The amount to add to the user's Player Attack.")]
    public float damageChange = 2f;

    [Tooltip("The factor to multiply the user's speed by")]
    public float speedChange = 0.25f;

    [Tooltip("How long the user should be stuck in place before the effect takes... effect.")]
    public float effectDelay = 1f;

    [Tooltip("How long the effect should last.")]
    public float effectDuration = 20f;

    [Tooltip("The amount of times the user can activate the effect per room.")]
    public int usesPerFight = 3;

    //The amount of uses this ability has left for this room
    private int usesRemaining = 0;

    //middleman variable for saving the user's change in speed
    private float totalSpeedChange = 0f;

    //Helper AAM to increase damage
    private ApatheticDamageAdd onAAM;

    //Helper AAM to decrease damage
    private ApatheticDamageSubtract offAAM;

    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        //initialize max uses
        usesRemaining = usesPerFight;

        //pre-build AAMs to lower lag at runtime
        onAAM = new ApatheticDamageAdd(damageChange);
        offAAM = new ApatheticDamageSubtract(damageChange);
    }

    /*The actual function of the ability. If it's not already applied and there are still
    uses remaining, activate the ability in ernest.*/
    protected override int InternInvoke(params int[] args)
    {
        usable = false;
        if(!user.myEffectHandler.EffectPresent<Apathetic>() && usesRemaining > 0)
        {
            StartCoroutine(PauseThenActivate());
            return 0;
        }
        return 1;
    }

    //Controls the beginning of the effect where the user can't move, the applies the effect
    IEnumerator PauseThenActivate()
    {
        //wait a little bit for the effect to take place
        yield return user.myMovement.LockActorMovement(effectDelay);

        //then, apply the effect and decrement remaining uses
        user.myEffectHandler.AddTimedEffect(this, effectDuration);
            
        --usesRemaining;
        
        StartCoroutine(coolDown(cooldownPeriod));
    }

    //The exectution of the effect; what happens when it is applied
    public bool ApplyEffect(ref Actor actor)
    {
        //augment the effectee's speed
        totalSpeedChange = actor.myMovement.speed * speedChange;
        actor.myMovement.speed -= totalSpeedChange;

        //THIS ONE'S THE SPECIFIC LINE
        //if this actor uses the player's attack...
        string playerAttackName = AbilityRegister.PLAYER_ATTACK;
        if(actor.myAbilityInitiator.abilities.ContainsKey(playerAttackName))
        {
            //then grab that ability and increase its damage
            onAAM.ModifyAbility(actor.myAbilityInitiator.abilities[playerAttackName]);
        }

        return true;
    }

    //The reverse of ApplyEffect, attempts to undo everything that was changed
    public void RemoveEffect(ref Actor actor)
    {
        //augment the effectee's speed
        actor.myMovement.speed += totalSpeedChange;

        //THIS ONE'S THE SPECIFIC LINE
        //if this actor uses the player's attack...
        string playerAttackName = AbilityRegister.PLAYER_ATTACK;
        if(actor.myAbilityInitiator.abilities.ContainsKey(playerAttackName))
        {
            //then grab that ability and increase its damage
            offAAM.ModifyAbility(actor.myAbilityInitiator.abilities[playerAttackName]);
        }
    }

    //AMM subclass for increasing the player's attack damage
    private class ApatheticDamageAdd : ActorAbilityModifier 
    {
        int damageChange = 0;
        //initializes changes dict as required
        public ApatheticDamageAdd (float damageChange) : base ()
        {
            this.damageChange = (int)damageChange;
            InitializeChanges(this.changes);
        }

        /*adds the needed change to the ability: increasing attack damage*/
        protected override void InitializeChanges(Dictionary<string, Action<dynamic>> changes)
        {
            Action<dynamic> del = new Action<dynamic> ( (dynamic arg) => {
                arg.AddDamage(damageChange);
            });

            changes.Add("AddDamage", del);
        }
    }

    //AMM subclass for decreasing the player's attack damage
    private class ApatheticDamageSubtract : ActorAbilityModifier 
    {
        int damageChange = 0;
        //initializes changes dict as required
        public ApatheticDamageSubtract (float damageChange) : base ()
        {
            this.damageChange = (int)damageChange;
            InitializeChanges(this.changes);
        }

        /*adds the needed change to the ability: decreases attack damage*/
        protected override void InitializeChanges(Dictionary<string, Action<dynamic>> changes)
        {
            Action<dynamic> del = new Action<dynamic> ( (dynamic arg) => {
                arg.AddDamage(-damageChange);
            });

            changes.Add("AddDamage", del);
        }
    }
}
