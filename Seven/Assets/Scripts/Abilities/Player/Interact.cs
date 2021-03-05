using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : ActorAbilityFunction<Actor, int>
{
    /*Similar to ActorAbilityFunction Invoke
    passes an actors movement component to InternalInvoke*/
    public override void Invoke(ref Actor user)
    {
        if (this.usable)
        {
            this.user = user;
            this.isFinished = false;
            InternInvoke(user);
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    public override void Invoke(ref Actor user, params object[] args)
    {
        Debug.LogWarning("Interact: Please utilize the \'Invoke(ref Actor)\' version of this method.");
    }

    /*InternInvoke performs a dodge on user's ActorMovement component*/
    protected override int InternInvoke(params Actor[] args)
    {
        if (Interactable.POTENTIAL_INTERACTABLE)
        {
            Interactable.POTENTIAL_INTERACTABLE.OnInteract();
        }
        this.isFinished = true;
        return 0;
    }
}
