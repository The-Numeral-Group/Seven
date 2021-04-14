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

    /*Starts the projectile!*/
    /*Making the rotation go isn't working right now, so I'm just gonna leave it for a second...*/
    /*public new void Launch(Vector2 target, LAUNCH_MODE mode = LAUNCH_MODE.POINT)
    {
        /*An explicit cast is added here, even though Vector3 implicitly converts
        to Vector2, to remove the ambiguity between subtracting the this.gameObject's position as
        a Vector3 or a Vector2*
        if(mode == LAUNCH_MODE.POINT)
        {
            launchDirection = (target - (Vector2)this.gameObject.transform.position).normalized;
        }
        else
        {
            launchDirection = target.normalized;
        }

        //rotate towards target. This is the only difference between this and the regular launch
        Vector3.RotateTowards(this.gameObject.transform.up, launchDirection, Mathf.PI * 2, 0f);

        //S H M O O V E
        moveFunction = new System.Action<Vector2>(InternalMovement);
    }*/

    //what happens when this projectile hits something
    protected override void OnTriggerEnter2D(Collider2D collided)
    {
        //only strike the target if it's not also a firewall
        if(!collided.gameObject.GetComponent<EgoFirewallProjectile>())
        {
            base.OnTriggerEnter2D(collided);
        }
    }

    /*What should happen every time the projectile moves (including the movement)
    Only move if this projectile hasn't crossed the max distance.*/
    protected override void InternalMovement(Vector2 movementDirection)
    {
        //We need absolute value, since magnitude might be negative depending on direction
        if(!TravelledTooFar())
        {
            mover.MoveActor(movementDirection);
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
        return Mathf.Abs(Vector3.Distance(this.gameObject.transform.position, starting_point)) > 
            firewallMaxDist;
    }
    

}
