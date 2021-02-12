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

    [Tooltip("Where the projectile should spawn relative to the user's faceAnchor.")]
    public Vector2 projectileScale = new Vector2(0.1f, 0.1f);

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
            waveObj = InstantiateProjectile(waveProjectile, user.faceAnchor, projectileScale);

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
            waveObj = InstantiateProjectile(waveProjectile, user.faceAnchor, projectileScale);
                //Instantiate(waveProjectile, user.faceAnchor.localPosition * projectileScale, Quaternion.identity);

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

    /*Creates the projectile object relative to the faceAnchor to make adjusting its spawn position
    with the inspector easier. Projectiles will still end and be launched parentless however,
    as their movement methods assume that projectiles have no parents.*/
    GameObject InstantiateProjectile(GameObject projPrefab, Transform faceAnchor, Vector2 offset)
    {
        //Instantiate the projectile as a child of faceAnchor
        GameObject obj = Instantiate(projPrefab, faceAnchor);

        //make it a child of faceAnchor's parent
        obj.transform.parent = faceAnchor.parent;

        //adjust projectile's position to match the faceAnchor's (with offset)
        obj.transform.localPosition = faceAnchor.localPosition * offset;

        //deparent the projectile
        obj.transform.parent = null;
        
        //return the projectile
        return obj;
    }
}
