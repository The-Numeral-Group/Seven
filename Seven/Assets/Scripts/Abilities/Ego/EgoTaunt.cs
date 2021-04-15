using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EgoTaunt : ActorAbilityFunction<float, int>, ActorEffect
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("The amount of time during which striking the user will trigger a counter.")]
    public float tauntDuration = 0.5f;

    [Tooltip("The amount of time the user will be stuck in place after a missed taunt.")]
    public float tauntVulnerableTime = 0.5f;

    [Tooltip("The attack to launch when the counter is triggered. WILL BE UNTARGETED.")]
    public ActorAbility COUNTER;

    //whether or not this ability is currently in taunt mode
    private bool taunting = false;

    //middleman vairable for remembering the user's damage resistance between taunts
    private float oldResist = 0f;

    //middleman variable for timing how long the recovery should be (as there
    //is no recovery time if the taunt lands correctly.
    private float tauntRecovery = tauntDuration;

    //METHODS--------------------------------------------------------------------------------------
    /*unlike most other ActorAbilities, EgoTaunt doesn't need custom 
    Invokes because it has no target*/
    
    //apply the Taunt effect and start the timer for its removal
    protected override int InternInvoke(params float args)
    {
        //apply the taunt effect
        user.myEffectHandler.AddTimedEffect(this, tauntDuration);

        return 0;
    }

    //what happens to the effectee when this target is applied
    public bool ApplyEffect(ref Actor actor)
    {
        //if the user isn't already taunting
        if(!user.myEffectHandler.EffectPresent<EgoTaunt>())
        {
            //freeze this actor's movement completely
            actor.myMovement.LockActorMovement(tauntDuration);

            //jack their damage resistance into the roof
            //we use this instead of setVulnerable because the user should still get hit
            oldResist = actor.myHealth.damageResistance;
            actor.myHealth.damageResistance = 1f;

            //and now we wait...
            taunting = true;
            return true;
        }
        
        //if it is, then abort the taunt
        isFinished = true;
        StartCoroutine(coolDown(cooldownPeriod));
        return false;
    }

    void RemoveEffect(ref Actor actor)
    {
        //end the taunt
        taunting = false;
        //make the user unsafe for a time
        StartCoroutine(TauntVulnerability(tauntRecovery));
        //finish the taunt
        isFinished = true;
        StartCoroutine(coolDown(cooldownPeriod));
    }

    IEnumerator TauntVulnerability(float duration)
    {
        //return user's health to normal
        user.myHealth.damageResistance = oldResistl;

        yield return new WaitForSeconds(duration);

        //unlock the actor's movement, in case it isn't already
        actor.myMovement.LockActorMovement(0f);
    }

    /*what happens when this actor gets damaged
    here's the fun part. The ActorEffect makes it easy to toggle
    this method on and off. If the user gets hit while taunting, they will
    IMMEDIATELY counterattack*/
    void DoActorDamageEffect()
    {
        if(taunting)
        {
            //immediately stop taunting
            tauntRecovery = 0.0f;
            user.myEffectHandler.SubtractEffect(this);
            tauntRecovery = tauntVulnerableTime;

            /*the only downside is that we don't know who hit the user, so we'll just
            need to shoot the counterattack out blindly, for now.*/
            COUNTER?.Invoke(ref user);
        }
    }
}
