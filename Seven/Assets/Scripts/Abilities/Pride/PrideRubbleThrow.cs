﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This class is just a simple ability wrapper/spawner for a projectile gameobject, which
does the actual shockwaving*/
public class PrideRubbleThrow : ActorAbilityFunction<GameObject, int>
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("The rubble projectile that this ability launches (the effects of the rubble" + 
        " can be configured by that game object).")]
    public GameObject rubbleProjectile;

    [Tooltip("Where the projectile should spawn relative to the user's faceAnchor.")]
    public Vector2 projectileScale = new Vector2(1, 1);

    //A middleman variable to hold the wave projectile between methods
    private GameObject rubbleObj;

    //METHODS--------------------------------------------------------------------------------------
    /*InternInvokes the ability as normal, but also instantiates a new RubbleObj to be launched by
    InternInvoke.*/
    public override void Invoke(ref Actor user)
    {
        if(usable)
        {
            isFinished = false;
            rubbleObj = InstantiateProjectile(rubbleProjectile, user.faceAnchor, projectileScale);
                //Instantiate(rubbleProjectile, user.faceAnchor.position * projectileScale, Quaternion.identity);

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
            rubbleObj = InstantiateProjectile(rubbleProjectile, user.faceAnchor, projectileScale);
                //Instantiate(rubbleProjectile, user.faceAnchor.position * projectileScale, Quaternion.identity);
            
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
            Debug.LogWarning("PrideRubbleThrow: can't launch projectile without a target");
            Destroy(rubbleObj);
            isFinished = true;

            return -1;
        }
        else
        {
            rubbleObj.GetComponent<PrideRubbleProjectile>()?.Launch(args[0].transform.position);
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
