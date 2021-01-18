using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostKnightProjectile : ActorAbilityFunction<Actor, int>
{
    public override IEnumerator coolDown(float cooldownDuration)
    {
        usable = false;
        yield return null;
    }
    public override void Invoke(ref Actor user)
    {

    }
    protected override int InternInvoke(params Actor[] args)
    {
        return 0;
    }
}
