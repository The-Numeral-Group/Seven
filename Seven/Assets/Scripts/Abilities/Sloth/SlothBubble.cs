using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlothBubble : ActorAbilityFunction<Vector3, int>
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("The prefab of the bubble game object.")]
    public GameObject bubblePrefab;

    //METHODS--------------------------------------------------------------------------------------
    /*Activates the ability with no arguments. In this case, it will default the bubble position
    to whereever the player is standing*/
    public override void Invoke(ref Actor user)
    {
        this.user = user;
        //by default, Invoke just does InternInvoke with no arguments
        if(usable)
        {
            isFinished = false;
            InternInvoke(GameObject.FindWithTag("Player").transform.position);
            StartCoroutine(coolDown(cooldownPeriod));
        }
        
    }

    /*Same as the above method, but with a provided vector position. Because this method always
    assumes a Vector3, no special conversion method is required*/
    public override void Invoke(ref Actor user, params object[] args)
    {
        this.user = user;
        //by default, Invoke just does InternInvoke with the provided arguments
        //it's also just implicitly convert the args and give it to InternInvoke
        if(usable)
        {
            isFinished = false;
            InternInvoke(easyArgConvert(args));
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    /*Creates the bubble at the target coordinates. The bubble takes care of the rest*/
    protected override int InternInvoke(params Vector3[] args)
    {
        Instantiate(bubblePrefab, args[0], Quaternion.identity);
        return 1;
    }
}
