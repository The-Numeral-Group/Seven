using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorAbilityCoroutine<InvokeParam> 
    : ActorAbilityFunction<InvokeParam, int>
{
    //METHODS--------------------------------------------------------------------------------------
    /*The standard InternInvoke, however it is sealed so that all it can do is start InternBuffer*/
    protected sealed override int InternInvoke(params InvokeParam[] args)
    {
        StartCoroutine(InternBuffer(args));
        return 0;
    }

    /*pivately manages the isFinished and coolDown of the ability, so that InternCoroutine doesn't
    have to care about either if it doesn't want to.*/
    IEnumerator InternBuffer(params InvokeParam[] args)
    {
        isFinished = false;
        yield return InternCoroutine(args);
        isFinished = true;
        StartCoroutine(coolDown(cooldownPeriod));
    }

    /*Like InternInvoke, but a coroutine. Very useful for timing-heavy abilities. Anything you'd
    put in InternInvoke should instead go in here.*/
    protected abstract IEnumerator InternCoroutine(params InvokeParam[] args);


}
