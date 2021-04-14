using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EgoFirewallProjectile : BasicProjectile
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("How far this firewall should travel before stopping.")]
    public float firewallMaxDist = 30f;

    [Tooltip("How long this firewall should persist after it stops.")]
    public float stoppedLifetime = 5f;

    //the coordinates of this object when it was launched, for judging when to stop.
    private Vector3 starting_point;

    //METHODS--------------------------------------------------------------------------------------
    // Invoked when this object is set to active for the first time
    void Awake()
    {
        starting_point = this.gameObject.transform.position;
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
        this.gameObject.transform.up = launchDirection;

        //S H M O O V E
        moveFunction = new System.Action<Vector2>(InternalMovement);

        //Begin lifetime
        StartCoroutine(Lifetime());
    }

    //what happens when this projectile hits something
    protected override void OnTriggerEnter2D(Collider2D collided)
    {
        //only strike the target if it's not also a firewall
        if(!collided.gameObject.GetComponent<EgoFirewallProjectile>())
        {
            base.OnTriggerEnter2D(collided);
        }
    }

    //Runs the lifetime of this projectile
    IEnumerator Lifetime()
    {
        //Step 1: wait until this projectile needs to stop moving
        //We need absolute value, since magnitude might be negative depending on direction
        yield return new WaitUntil( () => TravelledTooFar() );
        ///DEBUG
        Debug.Log("EgoFirewallProjectile: stopping");
        ///DEBUG
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
        var math = Mathf.Abs(Vector3.Distance(this.gameObject.transform.position, starting_point));
        bool result = math > 
            firewallMaxDist;
        //Debug.Log($"EgoFirewallProjectile: distance from start: {math}");
        if(result)
        {
            Debug.Log("EgoFirewallProjectile: I should stop now");
        }
        ///DEBUG
        return result;
    }
    

}
