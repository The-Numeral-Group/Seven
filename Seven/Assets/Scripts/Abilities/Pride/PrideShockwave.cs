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
    public GameObject waveProjectile;

    //[Tooltip("A target object to launch the shockwave at.")]
    //public GameObject target;

    //A middleman variable to hold the wave projectile between methods
    private GameObject waveObj;

    //METHODS--------------------------------------------------------------------------------------
    /*InternInvokes the ability as normal, but also instantiates a new waveObj to be launched by
    InternInvoke.*/
    public override void Invoke(ref Actor user)
    {
        if(usable)
        {
            isFinished = false;
            waveObj = 
                Instantiate(waveProjectile, user.gameObject.transform.position, Quaternion.identity);

            InternInvoke(GameObject.FindWithTag("Player"));
            StartCoroutine(coolDown(cooldownPeriod));
        }
        
    }

    public override void Invoke(ref Actor user, params object[] args)
    {
        if(usable)
        {
            GameObject target = null;

            isFinished = false;
            waveObj = 
                Instantiate(waveProjectile, user.faceAnchor.position, Quaternion.identity);

            if(args[0] is GameObject)
            {
                target = args[0] as GameObject;
            }
            else if(args[0] is MonoBehaviour)
            {
                target = (args[0] as MonoBehaviour).gameObject;
            }
            InternInvoke(target);
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    /*Launch the projectile. The anticipated argument is the gameObject being shot at. The 
    gameObject may-or-may-not have an ActorHealth component.*/
    protected override int InternInvoke(params GameObject[] args)
    {
        if(args[0] == null)
        {
            Debug.LogWarning("PrideShockwave: can't launch projectile without a target");
            Destroy(waveObj);
            isFinished = true;

            return -1;
        }
        else
        {
            waveObj.GetComponent<PrideWaveProjectile>()?.Launch(args[0].transform.position);
            isFinished = true;
        }
        return 0;
    }
}
