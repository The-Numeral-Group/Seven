using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAbility : ActorAbilityFunction<int, int>
{
    protected override int InternInvoke(params int[] args)
    {
        Debug.Log("DebugAbility fired");
        isFinished = true;
        return 0;
    }
}
