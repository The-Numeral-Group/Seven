using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EgoFirewallProjectile : BasicProjectile
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("How far this firewall should travel before stopping.")]
    public float firewallMaxDist = 150f;

    [Tooltip("How long this firewall should persist after it stops.")]
    public float stoppedLifetime = 2f;

    //the coordinates of this object when it was launched, for judging when to stop.
    private Vector3 starting_point;

    //METHODS--------------------------------------------------------------------------------------
    // Invoked when this object is set to active for the first time
    void Awake()
    {
        starting_point = this.gameObject.transform.position;
    }

    /*What should happen every time the projectile moves (including the movement)
    Only move if this projectile hasn't crossed the max distance.*/
    protected override void InternalMovement(Vector2 movementDirection)
    {
        //We need absolute value, since magnitude might be negative depending on direction
        var flightLength = 
            Mathf.Abs(Vector3.Distance(this.gameObject.transform.position, starting_point));
        if(flightLength < firewallMaxDist)
        {
            mover.MoveActor(movementDirection);
        }
    }

    //Runs the lifetime of this projectile
    IEnumerator Lifetime()
    {
        //Step 1: wait until this projectile needs to stop moving
        //We need absolute value, since magnitude might be negative depending on direction
        yield return new WaitUntil(
            () => Mathf.Abs(
                Vector3.Distance(this.gameObject.transform.position, starting_point)
            ) < firewallMaxDist
        );

        //Step 2: Wait some more time
        yield return new WaitForSeconds(stoppedLifetime);

        //Step 3: Destroy the projectile
        Cleanup();
    }

    void Cleanup()
    {
        Destroy(this.gameObject);
    }
    

}
