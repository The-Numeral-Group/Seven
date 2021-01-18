using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostKnightSlash : WeaponAbility
{
    protected override int InternInvoke(params Actor[] args)
    {
        StartCoroutine(args[0].myMovement.LockActorMovement(duration));
        base.InternInvoke(args);
        return 0;
    }
}
