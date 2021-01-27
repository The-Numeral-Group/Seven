using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This class is just a simple ability wrapper/spawner for a projectile gameobject, which
does the actual shockwaving*/
public class PrideRubbleThrow : ActorAbilityFunction<GameObject, int>
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("The rubble projectile that this ability launches (the effects of the rubble" + 
        " can be configured by that game object).")]
    public PrideRubbleProjectile rubbleProjectile;

    [Tooltip("A target object to launch the shockwave at.")]
    public GameObject target;

    //A middleman variable to hold the wave projectile between methods
    private GameObject rubbleObj;

    //METHODS--------------------------------------------------------------------------------------
    /*InternInvokes the ability as normal, but also instantiates a new waveObj to be launched by
    InternInvoke.*/
    public override void Invoke(ref Actor user)
    {
        if(usable)
        {
            isFinished = false;
            rubbleObj = 
                Instantiate(rubbleObj, user.gameObject.transform.position, Quaternion.identity);

            InternInvoke(target);
            StartCoroutine(coolDown(cooldownPeriod));
        }
        
    }
    /*Launch the projectile. The anticipated argument is the gameObject being shot at. The 
    gameObject may-or-may-not have an ActorHealth component.*/
    protected override int InternInvoke(params GameObject[] args)
    {
        rubbleObj.GetComponent<PrideRubbleProjectile>().Launch(args[0].transform.position);
        
        isFinished = true;

        return 0;
    }
}
