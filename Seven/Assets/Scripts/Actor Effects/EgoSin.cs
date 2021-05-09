using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EgoSin : ActorEffect
{
    //FIELDS---------------------------------------------------------------------------------------
    //The factor by which the user's speed should be multiplied
    private float speedBoost = 1f;

    //How long the effectee's invincibility will last
    private float duration = 5f;

    //The actual amount the user's speed will be increased by
    private float trueSpeedBoost;

    //Whether or not this instance should count applications for sin tracking
    private bool countSin;

    ///DEBUG
    //the effectee's orignal color
    private Color origColor = Color.white;
    ///DEBUG

    //The amount of times this sin has been applied during runtime
    //only incremented if the constructor recieves true
    public static int applicationCount { get; protected set; }

    //The amount of times this effect can stack. Readonly to prevent stack maxes from being
    //messed with at runtime.
    public static readonly int effectMaxStack = 1;
    //METHODS--------------------------------------------------------------------------------------
    public EgoSin(float speedBoost, float duration, bool countSin=true)
    {
        ///DEBUG
        Debug.Log("EgoSin: effect applied");
        ///DEBUG
        this.speedBoost = speedBoost;
        this.duration = duration;
        this.countSin = countSin;
    }
    
    //The actual application of this effect
    public bool ApplyEffect(ref Actor actor)
    {
        if(actor.myEffectHandler.EffectPresentCount<EgoSin>() < effectMaxStack)
        {
            //calculate and apply new speed
            var speedVal = actor.myMovement.speed;
            trueSpeedBoost = speedVal * (speedBoost - 1f);
            actor.myMovement.speed += trueSpeedBoost;

            //then make the effectee invincible
            actor.myHealth.SetVulnerable(false, duration, true);

            ///DEBUG
            //and turn them yellow
            var ren = actor.gameObject.GetComponent<SpriteRenderer>();
            if(ren)
            {
                origColor = ren.color;
                ren.color = Color.yellow;
            }
            ///DEBUG

            //count up the amount of times this effect has been applied
            if(countSin) {++EgoSin.applicationCount;}
            
            return true;
        }

        return false;
    }

    //ApplyEffect, but backwards to clean it off
    public void RemoveEffect(ref Actor actor)
    {
        //remove the speed boost
        actor.myMovement.speed -= trueSpeedBoost;

        ///DEBUG
        //and unyellow them
        var ren = actor.gameObject.GetComponent<SpriteRenderer>();
        if(ren)
        {
            ren.color = origColor;
        }
        ///DEBUG

        /*Invulnerability will not be cleaned, because there is no easy way to remove
        invulnerability without cancelling it entirely, and the effectee might have
        invulnerability from other sources.*/
    }
}
