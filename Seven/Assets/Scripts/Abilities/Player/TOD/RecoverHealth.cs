using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverHealth : ActorAbilityFunction<Actor, int>
{
    public float healAmount = 3f;

    void Start()
    {
        this.user = GetComponent<Actor>();
    }
    public override void Invoke(ref Actor user)
    {
        this.user = user;
        //by default, Invoke just does InternInvoke with no arguments
        if(usable && user.myHealth.currentHealth < user.myHealth.maxHealth)
        {
            isFinished = false;
            InternInvoke(new Actor[0]);
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    public override bool getUsable()
    {
        if (!this.user)
        {
            if (user.myHealth.currentHealth >= user.myHealth.maxHealth)
            {
                return false;
            }
        }
        return usable;
    }

    public override void Invoke(ref Actor user, params object[] args)
    {
        Invoke(ref user);
    }

    protected override int InternInvoke(params Actor[] args)
    {
        user.myHealth.currentHealth = 
            user.myHealth.currentHealth + 3 <= user.myHealth.maxHealth ?  
            user.myHealth.currentHealth + 3 : user.myHealth.maxHealth;
        this.isFinished = true;
        return 0;
    }
}
