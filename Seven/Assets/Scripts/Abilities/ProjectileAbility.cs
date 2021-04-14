using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAbility : ActorAbilityFunction<Vector2, int>
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("The projectile that this ability launches (the effects of the projectile" + 
        " can be configured by that game object).")]
    public GameObject projectile;

    [Tooltip("Where the projectile should spawn relative to the user's faceAnchor.")]
    public Vector2 projectileScale = new Vector2(1, 1);

    [Tooltip("Which direction the ability should be launched in if the user doesn't" + 
        " specify a Transform, Vector2 direction, Actor, or gameObject target")]
    public Vector2 projectileDirection = Vector2.left;

    //A middleman variable to hold the projectile instance between methods
    protected GameObject projObj;

    //A middleman variable to hold the desired LAUNCH_MODE between methods
    protected LAUNCH_MODE launchMode = LAUNCH_MODE.DIRECTION;

    // Start is called before the first frame update
    void Start()
    {
        if(projectile.GetComponent<BasicProjectile>() == null)
        {
            Debug.LogError("ProjectileAbility: the provided projectile gameObject" + 
                " does not have a BasicProjectile component.");
        }
    }

    //METHODS--------------------------------------------------------------------------------------
    /*InternInvokes the ability as normal, but also instantiates a new projObj to be launched by
    InternInvoke.*/
    public override void Invoke(ref Actor user)
    {
        this.user = user;
        if(usable)
        {
            isFinished = false;
            projObj = InstantiateProjectile(projectile, user.faceAnchor, projectileScale);

            InternInvoke(projectileDirection);
            StartCoroutine(coolDown(cooldownPeriod));
        }
        
    }

    public override void Invoke(ref Actor user, params object[] args)
    {
        this.user = user;
        if(usable)
        {
            isFinished = false;
            projObj = InstantiateProjectile(projectile, user.faceAnchor, projectileScale);
            if(args.Length > 1)
            {
                launchMode = ConvertMode(args[1]);
            }
            
            InternInvoke(ConvertTarget(args[0]));
            
            StartCoroutine(coolDown(cooldownPeriod));
        }
        
    }

    /*Launch the projectile. The anticipated argument is the gameObject being shot at. The 
    gameObject may-or-may-not have an ActorHealth component.*/
    protected override int InternInvoke(params Vector2[] args)
    {
        projObj.GetComponent<BasicProjectile>().Launch(args[0], launchMode);

        isFinished = true;

        return 0;
    }

    /*Creates the projectile object relative to the faceAnchor to make adjusting its spawn position
    with the inspector easier. Projectiles will still end and be launched parentless however,
    as their movement methods assume that projectiles have no parents.*/
    protected GameObject InstantiateProjectile(GameObject projPrefab, Transform faceAnchor, Vector2 offset)
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

    protected Vector2 ConvertTarget(object arg)
    {
        Vector2 finalVec = projectileDirection;

        if(arg is Vector2)
        {
            finalVec = (Vector2)arg;
        }
        else if(arg is Vector3)
        {
            finalVec = (Vector3)arg;
        }
        else if(arg is Transform)
        {
            finalVec = (arg as Transform).position;
        }
        else if(arg is GameObject)
        {
            finalVec = (arg as GameObject).transform.position;
        }
        else if(arg is Actor)
        {
            finalVec = (arg as Actor).gameObject.transform.position;
        }
        else
        {
            Debug.LogWarning("ProjectileAbility: provided argument cannot be" + 
                " converted/interpreted to Vector2. Using projectileDirection instead.");
        }
        
        return finalVec;
    }

    protected LAUNCH_MODE ConvertMode(object mode)
    {
        LAUNCH_MODE finalMode = LAUNCH_MODE.DIRECTION;

        if(mode is LAUNCH_MODE)
        {
            finalMode = (LAUNCH_MODE)mode;
        }
        else
        {
            Debug.LogWarning("ProjectileAbility: provided argument cannot be" + 
                " converted/interpreted to LAUNCH_MODE. Using LAUNCH_MODE.POINT instead.");
        }

        return finalMode;
    }
}
