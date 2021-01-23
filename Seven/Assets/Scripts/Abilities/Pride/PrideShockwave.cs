using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This class is just a simple ability wrapper/spawner for a projectile gameobject, which
does the actual shockwaving*/
public class PrideShockwave : ActorAbilityFunction<GameObject, int>
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("The shockwave projectile that this ability launches (the effects of the shockwave" + 
        " can be configured by that game object).")]
    public PrideWaveProjectile waveProjectile;

    [Tooltip("A target object to launch the shockwave at.")]
    public GameObject target;

    //A middleman variable to hold the wave projectile between methods
    private GameObject waveObj;

    //METHODS--------------------------------------------------------------------------------------
    /*InternInvokes the ability as normal, but also instantiates a new waveObj to be launched by
    InternInvoke.*/
    public override void Invoke(ref Actor user)
    {
        //by default, Invoke just does InternInvoke with no arguments
        if(usable)
        {
            isFinished = false;
            waveObj = 
                Instantiate(waveObj, user.gameObject.transform.position, Quaternion.identity);

            InternInvoke(target);
            StartCoroutine(coolDown(cooldownPeriod));
        }
        
    }
    /*Launch the projectile. The anticipated argument is the gameObject being shot at. The 
    gameObject may-or-may-not have an ActorHealth component.*/
    protected override int InternInvoke(params GameObject[] args)
    {
        waveObj.GetComponent<PrideWaveProjectile>().Launch(args[0].transform.position);
        
        isFinished = true;

        return 0;
    }
}
