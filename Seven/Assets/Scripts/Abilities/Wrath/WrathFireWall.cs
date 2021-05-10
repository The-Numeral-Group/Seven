﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathFireWall : ActorAbilityFunction<Actor, int>
{
    public override void Invoke(ref Actor user)
    {
        if (usable)
        {
            isFinished = false;
            InternInvoke(user);
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }
    protected override int InternInvoke(params Actor[] args)
    {
        //StartCoroutine(args[0].myMovement.LockActorMovementOnly(this.duration));
        StartCoroutine(FireWallFinished(args[0]));
        return 0;
    }

    private IEnumerator FireWallFinished(Actor user)
    {
        yield return new WaitForSeconds(0.0f);
        user.myMovement.DragActor(new Vector2(0.0f, 0.0f));
        isFinished = true;
    }
}