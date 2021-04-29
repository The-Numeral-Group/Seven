using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ActorHealth))]
public class EgoFirewallCompositeProjectile : BasicProjectile
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("How far this firewall should travel before stopping.")]
    public float firewallMaxDist = 30f;

    [Tooltip("How long this firewall should persist after it stops.")]
    public float stoppedLifetime = 5f;

    [Tooltip("How far apart individual fires should be.")]
    public float fireSpread = 10f;

    //the coordinates of this object when it was launched, for judging when to stop.
    private Vector3 starting_point;

    //the coordinates of the last place this projectile made a fire
    private Vector3 lastFireSpot;

    //the distance between fire objects. Defaults to the largets dimension of the fire's bounds
    private float distBetweenFires;

    //METHODS--------------------------------------------------------------------------------------
    // Invoked when this object is set to active for the first time
    void Awake()
    {
        starting_point = this.gameObject.transform.position;
        lastFireSpot = this.gameObject.transform.position;
    }

    //Invoked before the first update frame
    //needed to access this object's collider, which might not be ready in awake
    protected override void Start()
    {
        base.Start();
        var colBounds = this.gameObject.GetComponent<Collider2D>().bounds.extents;
        distBetweenFires = (colBounds.x > colBounds.y ? colBounds.x : colBounds.y) + fireSpread;
    }

    //what to do when this actor's health reaches 0
    public void DoActorDeath()
    {
        Cleanup();
    }

    /*//rotate towards target. This is the only difference between this and the regular launch
        //Vector3.RotateTowards(this.gameObject.transform.up, launchDirection, Mathf.PI * 2, 0f);
        
            //Vector3.Lerp(this.gameObject.transform.up, launchDirection, 1); 

        */
    /*Starts the projectile! Also rotates the firewall and starts its lifetime*/
    public override void Launch(Vector2 target, LAUNCH_MODE mode = LAUNCH_MODE.POINT)
    {
        /*An explicit cast is added here, even though Vector3 implicitly converts
        to Vector2, to remove the ambiguity between subtracting the this.gameObject's position as
        a Vector3 or a Vector2*/
        if(mode == LAUNCH_MODE.POINT)
        {
            launchDirection = (target - (Vector2)this.gameObject.transform.position).normalized;
        }
        else
        {
            launchDirection = target.normalized;
        }

        //hard rotate the firewall
        //this.gameObject.transform.up = launchDirection;

        //S H M O O V E
        moveFunction = new System.Action<Vector2>(InternalMovement);

        //Begin lifetime
        StartCoroutine(Lifetime());
    }

    //what happens when this projectile hits something
    protected override void OnTriggerEnter2D(Collider2D collided)
    {
        //only strike the target if it's not also a firewall
        if(!collided.gameObject.GetComponent<EgoFirewallCompositeProjectile>())
        {
            base.OnTriggerEnter2D(collided);
        }
    }

    //allow outside objects to force this object to stop moving
    public void StopFlight()
    {
        StartCoroutine(InternStop());
    }

    //Runs the lifetime of this projectile
    IEnumerator Lifetime()
    {
        //Step 1: wait until this projectile needs to stop moving
        //We need absolute value, since magnitude might be negative depending on direction
        yield return new WaitUntil( () => TravelledTooFar() );
        ///DEBUG
        Debug.Log("EgoFirewallCompositeProjectile: stopping");
        ///DEBUG
        StartCoroutine(InternStop());
    }

    IEnumerator InternStop()
    {
        this.moveFunction = null;
        //this line prevents the wall from being pushed by other things
        this.gameObject.GetComponent<Rigidbody2D>().constraints 
            = RigidbodyConstraints2D.FreezeAll;

        //Step 2: Wait some more time
        yield return new WaitForSeconds(stoppedLifetime);

        //Step 3: Destroy the projectile
        Cleanup();
    }

    void Cleanup()
    {
        Destroy(this.gameObject);
    }

    //Returns whether this projectile has travelled farther than it should have
    bool TravelledTooFar()
    {
        ///DEBUG
        var totalMath = 
            Mathf.Abs(Vector3.Distance(this.gameObject.transform.position, starting_point));
        var fireMakeMath = 
            Mathf.Abs(Vector3.Distance(this.gameObject.transform.position, lastFireSpot));
        bool result = totalMath > firewallMaxDist;
        
        //if this projectile is far enough from the last fire it made...
        if(fireMakeMath > distBetweenFires)
        {
            //make a new fire, but immediately stop it
            //new fires are identical copies of this object...
            Debug.Log("EgoFirewallCompositeProjectile: Making new fire...");
            Instantiate(
                this.gameObject, 
                this.gameObject.transform.position, 
                Quaternion.identity
            ).GetComponent<EgoFirewallCompositeProjectile>().StopFlight();
            lastFireSpot = this.gameObject.transform.position;
        }

        ///DEBUG
        if(result)
        {
            Debug.Log("EgoFirewallCompositeProjectile: I should stop now");
        }
        ///DEBUG

        return result;
    }
    

}