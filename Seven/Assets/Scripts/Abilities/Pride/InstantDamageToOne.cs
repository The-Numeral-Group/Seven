using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDamageToOne : ActorAbilityFunction<ActorHealth, int>
{
    //This ability has no defined function with no target
    public override void Invoke(ref Actor user)
    {
        Debug.LogWarning("InstantDamageToOne: This ability has no defined behavior" + 
            " with no provided target.");
    }
    
    public override void Invoke(ref Actor user, params object[] args)
    {
        //by default, Invoke just does InternInvoke with the provided arguments
        //it's also just implicitly convert the args and give it to InternInvoke
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

    protected override int InternInvoke(params ActorHealth[] args)
    {
        var healthEffected = 0;
        foreach(ActorHealth health in args)
        {
            if(health.currentHealth > 1f)
            {
                health.takeDamage(health.currentHealth - 1f);
            }
            else
            {
                health.takeDamage(1f);
            }
            ++healthEffected;
        }

        return healthEffected;
    }
}
