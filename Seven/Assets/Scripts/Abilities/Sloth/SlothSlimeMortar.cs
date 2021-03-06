using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlothSlimeMortar : ActorAbilityFunction<ActorMovement, int>
{
    [Tooltip("The marker object to insantiate for the ability. Will be placed where the" + 
        " projectile will land.")]
    public GameObject groundMarkerPrefab;

    [Tooltip("The projectile that this ability launches (the effects of the projectile" + 
        " can be configured by that game object).")]
    public GameObject projectilePrefab;

    [Tooltip("Where the projectile should spawn relative to the user's faceAnchor.")]
    public Vector2 projectileScale = new Vector2(1, 1);

    [Tooltip("The amount of time in SECONDS it would theoretically take the target to move from" + 
        " their current position to the position of the second marker if they never changed" + 
            " speed or direction (this influences how far the second marker is from the first" + 
                " one, which starts underneath the target.")]
    public float temporalOffset = 1f;

    //the two shots that are going to be fired directly at their markers
    //private Tuple<GameObject, GameObject> directShots;

    //the two shots that are going to be fired in an arc at their markers
    //private Tuple<GameObject, GameObject> arcShots;

    //the marker to place under the target's position
    private GameObject positionMarker;

    //the marker to place where it is believed the target is going to be
    private GameObject leadMarker;

    //A middleman variable to hold the desired LAUNCH_MODE between methods
    //private LAUNCH_MODE launchMode = LAUNCH_MODE.POINT;

    // Start is called before the first frame update
    void Start()
    {
        if(projectilePrefab.GetComponent<BasicProjectile>() == null)
        {
            Debug.LogError("SlothSlimeMortar: the provided projectile gameObject" + 
                " does not have a BasicProjectile component.");
        }

        if(projectilePrefab.GetComponent<SlothRangeMarker>() == null)
        {
            Debug.LogError("SlothSlimeMortar: the provided marker gameObject" + 
                " does not have a SlothRangeMarger component.");
        }
    }

    /*Invokes the projectiles, assuming to use the Player as the approprite target*/
    public override void Invoke(ref Actor user)
    {
        this.user = user;
        //by default, Invoke just does InternInvoke with no arguments
        if(usable)
        {
            var target = GameObject.FindWithTag("Player")?.GetComponent<ActorMovement>();

            InstantiateMarkers(groundMarkerPrefab, target);

            isFinished = false;
            InternInvoke(target);
            StartCoroutine(coolDown(cooldownPeriod));
        }
        
    }

    /*Same as the above method, but this overload allows an arbitrary number of
    Object objects to be passed in to assist the ability. It's up to the ability
    to figure out what to do with these additional arguments, and what to do if
    it gets something it doesn't expect.*/
    public override void Invoke(ref Actor user, params object[] args)
    {
        this.user = user;
        //by default, Invoke just does InternInvoke with the provided arguments
        //it's also just implicitly convert the args and give it to InternInvoke
        if(usable)
        {
            isFinished = false;
            InternInvoke(ConvertTarget(args[0]));
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    /*Invokes the actual ability. The projectiles will calculate how they move on their own, so
    this method only needs to instantiate them, give them markers, and launch them.
    This method and its formatting fucking sucks, but each projectile needs a different instantiation,
    so yeah that's life.*/
    protected override int InternInvoke(params ActorMovement[] args)
    {
        //first, make all the projectiles.
        //this is just a helper variable
        SlothRangeProjectile behaviour;

        var directPosition = InstantiateProjectile(projectilePrefab, user.faceAnchor, projectileScale);
        var directLead = InstantiateProjectile(projectilePrefab, user.faceAnchor, projectileScale);
        var arcPosition = InstantiateProjectile(projectilePrefab, user.faceAnchor, projectileScale);
        var arcLead = InstantiateProjectile(projectilePrefab, user.faceAnchor, projectileScale);

        //starting with the directs...
        //the first one...
        behaviour = directPosition.GetComponent<SlothRangeProjectile>();
        //the first item flys towards the target, and both directShots fly straight
        behaviour.marker = positionMarker.GetComponent<SlothRangeMarker>();
        behaviour.movementNature = SlothRangeProjectile.SlothProjectileNature.STRAIGHT;

        //the second one...
        behaviour = directLead.GetComponent<SlothRangeProjectile>();
        //the second item flys towards where the target will be, and both directShots fly straight
        behaviour.marker = leadMarker.GetComponent<SlothRangeMarker>();
        behaviour.movementNature = SlothRangeProjectile.SlothProjectileNature.STRAIGHT;

        //now the arcs
        //the first one
        behaviour = arcPosition.GetComponent<SlothRangeProjectile>();
        //the first item flys towards the target, and both arcShots fly in an arc
        behaviour.marker = positionMarker.GetComponent<SlothRangeMarker>();
        behaviour.movementNature = SlothRangeProjectile.SlothProjectileNature.ARC;

        //the second one...
        behaviour = arcLead.GetComponent<SlothRangeProjectile>();
        //the second item flys towards where the target will be, and both arcShots fly in an arc
        behaviour.marker = leadMarker.GetComponent<SlothRangeMarker>();
        behaviour.movementNature = SlothRangeProjectile.SlothProjectileNature.ARC;

        //and now that they're all set, launch ALL of them! At the same time!
        directPosition.GetComponent<SlothRangeProjectile>()
            .Launch(positionMarker.transform.position, LAUNCH_MODE.POINT);

        directLead.GetComponent<SlothRangeProjectile>()
            .Launch(leadMarker.transform.position, LAUNCH_MODE.POINT);

        arcPosition.GetComponent<SlothRangeProjectile>()
            .Launch(positionMarker.transform.position, LAUNCH_MODE.POINT);

        arcLead.GetComponent<SlothRangeProjectile>()
            .Launch(leadMarker.transform.position, LAUNCH_MODE.POINT);

        //god this method sucks! Hopefully I'll go back and make it not garbage!

        return 4;
        
    }

    /*Calculates the needed positions of the markers and places them in the scene*/
    void InstantiateMarkers(GameObject markerPrefab, ActorMovement target)
    {
        //the first marker is easy, it just goes where the target is.
        positionMarker = Instantiate(
            markerPrefab, 
            target.gameObject.transform.position, 
            Quaternion.identity
        );

        /*the second marker needs to be placed where the target is going to be in temporalOffset
        seconds. Thankfully, ActorMovements hold both speed and direction. Since the script starts 
        with time, the only value that must be calculated is the distance offset.*/
        /*I (Thomas) am calculating this by normalizing the AcotrMovement's moveDirection and
        multiplying it by a standard d = rt, where r is the ActorMovement's speed and t is
        temporalOffset*/
        Vector2 distanceVec = target.movementDirection.normalized * (target.speed * temporalOffset);

        /*I (Thomas) am then going to get the final position by adding this distance vector to the
        position of the first marker i.e. to the target's current position*/
        leadMarker = Instantiate(
            markerPrefab, 
            target.gameObject.transform.position + new Vector3(distanceVec.x, distanceVec.y, 0), 
            Quaternion.identity
        );

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

    ActorMovement ConvertTarget(object arg)
    {
        ActorMovement finalVec = null;

        if(arg is ActorMovement)
        {
            finalVec = (ActorMovement)arg;
        }
        else if(arg is GameObject)
        {
            finalVec = (arg as GameObject).GetComponent<ActorMovement>();
        }
        else if(arg is Actor)
        {
            finalVec = (arg as Actor).myMovement;
        }
        else
        {
            Debug.LogWarning("SlothSlimeMortar: provided argument cannot be" + 
                " converted/interpreted to ActorMovement. Using null instead.");
        }
        
        return finalVec;
    }
}
