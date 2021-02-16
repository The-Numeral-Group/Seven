using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDamageToOne : ActorAbilityFunction<ActorHealth, int>
{
    //METHODS--------------------------------------------------------------------------------------
    //This ability has no defined function with no target
    public override void Invoke(ref Actor user)
    {
        Debug.LogWarning("InstantDamageToOne: This ability has no defined behavior" + 
            " with no provided target.");
    }
    
    /*Activates the ability, and translates provided arguments as approprite*/
    public override void Invoke(ref Actor user, params object[] args)
    {
        /*We attempt to cast every argument as an ActorHealth OR try to pull
        an ActorHealth from and Actor object. Keep in mind that ActorHealth doesn't
        have a provided default value, so unconvertable arguments will be null*/
        if(usable)
        {
            isFinished = false;
            var ah = System.Array.ConvertAll(args, (x) => {
                if(x is ActorHealth y)
                {
                    return y;
                }
                else if(x is Actor z)
                {
                    return z.myHealth;
                }
                else
                {
                    return default(ActorHealth);
                }
            });
            InternInvoke(ah);
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    /*The actual ability. The math here's pretty simple: either take off enough health
    to leave 1 HP, or do 1HP for the kill. Since not every argument will have a value,
    the amount of health objects that are effected is tracked. Nothing is done with this
    information, but it might be useful later...*/
    protected override int InternInvoke(params ActorHealth[] args)
    {
        var healthEffected = 0;
        foreach(ActorHealth health in args)
        {
            if(health && health.currentHealth > 1f)
            {
                health.takeDamage(health.currentHealth - 1f);
                ++healthEffected;
            }
            else
            {
                health.takeDamage(1f);
                ++healthEffected;
            }
        }

        return healthEffected;
    }
}
