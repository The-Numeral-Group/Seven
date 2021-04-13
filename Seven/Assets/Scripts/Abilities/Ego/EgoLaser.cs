using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EgoLaser : ActorAbilityFunction<Vector3, int>
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("A prefab to use as the laser projectile (must have a line renderer component).")]
    public GameObject laserObj;

    [Tooltip("How long the laser should be if it doesn't hit anything")]
    public float laserMaxDist = 100f;

    [Tooltip("How wide the laser should be.")]
    public float laserWidth = 3.0f;

    [Tooltip("How much damage the laser should do.")]
    public float damage = 2f;

    [Tooltip("How long the laser should be shown before it starts to damage things.")]
    public float preLaserDuration = 0.5f;

    [Tooltip("How long the laser should be shown at full power (this is not how long the" + 
        " hitbox lasts).")]
    public float laserDuration = 0.3f;

    /*[Tooltip("How long the ability should linger (holding up its user). If this number is " + 
        " too low, the user might move or some other thing before the laser is destroyed.")]
    public float laserEndDuration = 0.15f;*/
    
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
            //StartCoroutine(coolDown(cooldownPeriod));
        }
        
    }

    /*Same as the above method, but with a provided vector position*/
    public override void Invoke(ref Actor user, params object[] args)
    {
        this.user = user;
        //by default, Invoke just does InternInvoke with the provided arguments
        //it's also just implicitly convert the args and give it to InternInvoke
        if(usable)
        {
            isFinished = false;
            InternInvoke(ActorAbilityFunction<Vector3, int>.ConvertTargetToVec3(args[0]));
            //StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    //wrapper for a coroutine that handles the duration of the actual laser
    protected override int InternInvoke(params Vector3[] args)
    {
        usable = false;
        StartCoroutine(LaserInvokation(args[0]));
        return 1;
    }

    //launches the laser, and turns on its damage when the time comes
    IEnumerator LaserInvokation(Vector3 targetPoint)
    {
        //Step 1: figure out the direction to the target
        Vector3 targetDirection = (targetPoint - user.gameObject.transform.position).normalized;

        //Step 2: create a laser object and attach the EgoLaserProjectile component
        var laser = Instantiate(laserObj, this.gameObject.transform)
            .AddComponent<EgoLaserProjectile>();
        
        //Step 3: initialize the laser
        laser.Initialize(laserMaxDist, laserWidth, damage);

        //Step 4: fire the prelaser
        laser.ShootLaser(user.faceAnchor.position, targetDirection);

        //Step 5: wait a little bit
        yield return new WaitForSeconds(preLaserDuration);

        //Step 6: fire the actual laser
        laser.ShootLaser(user.faceAnchor.position, targetDirection, true);

        //Step 7: wait a little bit
        yield return new WaitForSeconds(laserDuration);

        //Step 8: destroy the laser and begin cooldown
        laser.gameObject.SetActive(false);
        Destroy(laser.gameObject);

        /*Step 8.5: yield before setting to isFinished, 
        so the destruction of the laser has time to appear*
        //actually disable it then destroy it
        //because the destruction will always wait for the next physics tick
        yield return new WaitForSeconds(laserEndDuration);*/
        isFinished = true;
        
        //Step 9: cooldown
        StartCoroutine(coolDown(cooldownPeriod));
    }
}

internal class EgoLaserProjectile : MonoBehaviour
{
    //FIELDS---------------------------------------------------------------------------------------
    //how far the laser should travel before just stopping
    private float laserMaxDistance = 50f;

    //how much damage the laser will deal
    private float laserDamage = 2f;

    //the lineRenderer that handles the graphics of the laser
    private LineRenderer line;

    //METHODS--------------------------------------------------------------------------------------
    void Awake()
    {
        line = this.gameObject.GetComponent<LineRenderer>();
    }

    //set certain values for the laser, since MonoBehaviour constructors are inconsistent
    public void Initialize(float maxDist=50f, float width=1f, float damage=2f)
    {
        laserMaxDistance = maxDist;
        laserDamage = damage;

        line.startWidth = width;
        line.endWidth = width;
    }

    /*Actually draw the laser. If the shot is active, 
    the laser will also attempt to damage whatever got hit*/
    public void ShootLaser(Vector3 launchPoint, Vector3 launchDirection, bool active=false)
    {
        //prep vector end points
        Vector3 laserStart = launchPoint;
        Vector3 laserEnd;

        //shoot what is effectively a data laser
        RaycastHit2D hit = Physics2D.Raycast(launchPoint, launchDirection, laserMaxDistance);

        /*//if it hit something...
        if(hit.collider != null)
        {
            //then the laser ends at that point
            laserEnd = hit.point;
        }
        //if not...
        else
        {
            //the laser ends some distance away in that direction
            laserEnd = launchDirection * laserMaxDistance;
        }*/

        //draw the laser the whole way regardless
        laserEnd = launchDirection * laserMaxDistance;

        //set the points for the laser
        //what C# doesn't have implicit arrays? Really?
        Vector3[] laserPoints = new Vector3[] {laserStart, laserEnd};
        line.SetPositions(laserPoints);

        //if the laser is active
        if(active)
        {
            //try to hurt the target
            hit.collider?.gameObject.GetComponent<ActorHealth>()?.takeDamage(laserDamage);

            ///DEBUG
            //switch the laser's color to red just to see it for right now
            line.startColor = Color.red;
            line.endColor = Color.red;
            ///DEBUG
        }
    }
}
