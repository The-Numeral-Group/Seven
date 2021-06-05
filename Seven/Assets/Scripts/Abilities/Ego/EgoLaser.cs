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

    //the last laser to be used
    private EgoLaserProjectile laser;

    //the coroutine of the laser invokation
    private Coroutine laserTiming;

    //[Tooltip("How far the laser beam should be from the user's faceAnchor")]
    //public Vector2 laserOffset = Vector2.zero;

    /*[Tooltip("How long the ability should linger (holding up its user). If this number is " + 
        " too low, the user might move or some other thing before the laser is destroyed.")]
    public float laserEndDuration = 0.15f;*/
    
    
    //METHODS--------------------------------------------------------------------------------------
    /*Activates the ability with no arguments. In this case, it will default the target position
    to the direction the user is facing*/
    public override void Invoke(ref Actor user)
    {
        this.user = user;
        //by default, Invoke just does InternInvoke with no arguments
        if(usable)
        {
            isFinished = false;
            InternInvoke(user.faceAnchor.position);
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
        
        
        laserTiming = StartCoroutine(LaserInvokation(args[0]));
        return 1;
    }

    //launches the laser, and turns on its damage when the time comes
    IEnumerator LaserInvokation(Vector3 targetPoint)
    {

        //Step 1: figure out the direction to the target
        Vector3 targetDirection = (targetPoint - user.gameObject.transform.position).normalized;

        //Step 1.5: face the user in that direction
        //user.gameObject.SendMessage("DoActorUpdateFacing", targetDirection);

        //Step 2: create a laser object and attach the EgoLaserProjectile component
        //var laser = Instantiate(laserObj, user.faceAnchor.position, 
            //Quaternion.identity).AddComponent<EgoLaserProjectile>();
        laser = Instantiate(laserObj, user.gameObject.transform)
            .AddComponent<EgoLaserProjectile>();
        //but set it's direction towards the player manually
        //needs to be offset by 45 degrees because the laser asset is rotated that way
        laser.gameObject.transform.localPosition = user.faceAnchor.localPosition;// * laserOffset;
        laser.gameObject.transform.up = targetDirection; //+ new Vector3(-45f, 0f, 0f);
        
        //Step 3: initialize the laser
        laser.Initialize(laserMaxDist, laserWidth, damage);

        //Step 2.5: lock the user's movement
        var moveStop = user.myMovement.LockActorMovement(Mathf.Infinity);
        StartCoroutine(moveStop);

        //Step 4: fire the prelaser
        laser.ShootLaser(user.faceAnchor.position, targetDirection);

        //Step 5: wait a little bit
        yield return new WaitForSeconds(preLaserDuration);
        //Lock the user's movement during this time...
        //yield return user.myMovement.LockActorMovement(preLaserDuration);

        //Animate the attack now for when the laser exists
        var attackanim = user.myAnimationHandler.TryFlaggedSetTrigger("ego_shoot");

        //wait a magical number of seconds
        yield return new WaitForSeconds(0.55f);

        //Step 6: fire the actual laser
        StartCoroutine(laser.CastDamage(user.faceAnchor.localPosition, targetDirection));

        //Step 7: wait a little bit longer
        yield return new WaitForSeconds(laserDuration);

        //Step 8: destroy the laser and begin cooldown
        laser.gameObject.SetActive(false);
        Destroy(laser.gameObject);

        /*Step 8.5: yield before setting to isFinished, 
        so the destruction of the laser has time to appear*
        //actually disable it then destroy it
        //because the destruction will always wait for the next physics tick
        yield return new WaitForSeconds(laserEndDuration);*/
        yield return new WaitWhile(attackanim);
        isFinished = true;

        //Step 8.75: unlock the user's movement
        StopCoroutine(moveStop);
        StartCoroutine(user.myMovement.LockActorMovement(0f));
        
        //Step 9: cooldown
        StartCoroutine(coolDown(cooldownPeriod));
    }

    /*update is called once per frame
    it is used here to check if the user of the laser is still alive. If they aren't, the beam
    should be destroyed*/
    void Update()
    {
        //if the ability is being used but there is no user...
        if(!isFinished && !user)
        {
            //stop the launch sequence
            StopCoroutine(laserTiming);
            //and destroy the laser, if there is one
            Destroy(laser?.gameObject);
        }
    }
}

internal class EgoLaserProjectile : MonoBehaviour
{
    //FIELDS---------------------------------------------------------------------------------------
    //how far the laser should travel before just stopping
    private float laserMaxDistance = 50f;

    //how much damage the laser will deal
    private float laserDamage = 2f;

    //how wide the laser is
    private float width = 4f;

    //the lineRenderer that handles the graphics of the laser
    //private LineRenderer line;

    //METHODS--------------------------------------------------------------------------------------
    /*void Awake()
    {
        line = this.gameObject.GetComponent<LineRenderer>();
    }*/

    //set certain values for the laser, since MonoBehaviour constructors are inconsistent
    public void Initialize(float maxDist=50f, float width=1f, float damage=2f)
    {
        laserMaxDistance = maxDist;
        laserDamage = damage;

        this.width = width;

        //also play the chargeup sound
        this.gameObject.GetComponent<ActorSoundManager>().PlaySound("laser_charge");
    }

    //Handles the literal boxCast of the 
    public IEnumerator CastDamage(Vector3 launchPoint, Vector3 damageDirection)
    {
        this.gameObject.GetComponent<Animator>().SetTrigger("go");
        
        //set the points for the laser
        //what C# doesn't have implicit arrays? Really?
        //Vector3[] laserPoints = new Vector3[] {laserStart, laserEnd};
        //line.SetPositions(laserPoints);

        //if the laser is active
        /*if(active)
        {
            //shoot what is effectively a really thicc data laser
            RaycastHit2D[] hits = Physics2D.BoxCastAll(
                launchPoint,                                    //start of box
                new Vector2 (width, width),   //size of box
                Vector3.Angle(Vector3.zero, damageDirection),   //rotation of box
                damageDirection,                                //direction of box "movement"
                laserMaxDistance                                //travel distance of box
            );

            //try to hurt anything caught in the laser's path
            foreach(RaycastHit2D hit in hits)
            {
                hit.collider?.gameObject.GetComponent<ActorHealth>()?.takeDamage(laserDamage);
            }
            //try to hurt the target
            //hit.collider?.gameObject.GetComponent<ActorHealth>()?.takeDamage(laserDamage);

            ///DEBUG
            //switch the laser's color to red just to see it for right now
            //line.startColor = Color.red;
            //line.endColor = Color.red;
            ///DEBUG
        }*/

        ///DEBUG
        //this line represents this laser's length. It will not show up in game
        Debug.DrawLine(
            launchPoint + (damageDirection * width), 
            launchPoint + (damageDirection * laserMaxDistance), 
            Color.red, 
            10.0f
        );
        ///DEBUG

        yield return new WaitForSeconds(0.25f);
        var sound = this.gameObject.GetComponent<ActorSoundManager>();
        sound.StopSound("laser_charge");
        sound.PlaySound("laser_fire");

        //shoot what is effectively a really thicc data laser
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            launchPoint + (damageDirection * width),        //start of box
            new Vector2 (width, width),                     //size of box
            Vector3.Angle(Vector3.zero, damageDirection),   //rotation of box
            damageDirection,                                //direction of box "movement"
            laserMaxDistance                                //travel distance of box
        );

        //try to hurt anything caught in the laser's path
        foreach(RaycastHit2D hit in hits)
        {
            hit.collider?.gameObject.GetComponent<ActorHealth>()?.takeDamage(laserDamage);
        }
            //try to hurt the target
            //hit.collider?.gameObject.GetComponent<ActorHealth>()?.takeDamage(laserDamage);

            ///DEBUG
            //switch the laser's color to red just to see it for right now
            //line.startColor = Color.red;
            //line.endColor = Color.red;
            ///DEBUG
    }

    /*Actually draw the laser. If the shot is active, 
    the laser will also attempt to damage whatever got hit*/
    public void ShootLaser(Vector3 launchPoint, Vector3 launchDirection, bool active=false)
    {
        //prep vector end points
        Vector3 laserStart = launchPoint;
        Vector3 laserEnd;

        //draw the laser the whole way regardless
        laserEnd = launchDirection * laserMaxDistance;

        //CastDamage(launchDirection);
        
        //doesnt do anything, since visuals are handled by an animator now
    }
}
