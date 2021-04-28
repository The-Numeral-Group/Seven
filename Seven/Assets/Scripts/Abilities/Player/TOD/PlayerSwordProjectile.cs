using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordProjectile : BasicProjectile
{
    //FIELDS---------------------------------------------------------------------------------------
    //how far the sword should travel before stopping
    private float travelDistance = 5f;

    //the sword's starting point for judging distance calculations
    private Vector3 startingPoint;

    //the wrapper, in case the sword needs to tell the wrapper that it has been picked up
    private TossAndTeleport wrapper;

    //METHODS--------------------------------------------------------------------------------------
    void Awake()
    {
        //the sword should never be destroyed if it hits something
        this.destroyOnHit = false;
    }

    //Does the normal launch with a default distance of 5 and damage of 2
    public override void Launch(Vector2 target, LAUNCH_MODE mode = LAUNCH_MODE.POINT)
    {
        travelDistance = 5f;
        damage = 2f;
        startingPoint = this.gameObject.transform.position; 

        base.Launch(target, mode);
    }

    /*Starts the projectile! Hides the OG launch in exchange for a user defined travel distance*
    and damage value*/
    public void Launch(Vector2 target, LAUNCH_MODE mode = LAUNCH_MODE.POINT, 
        TossAndTeleport wrapper=null)
    {
        this.travelDistance = wrapper.travelDistance;
        this.damage = wrapper.damage;  
        this.wrapper = wrapper;
        startingPoint = this.gameObject.transform.position;

        base.Launch(target, mode);
    }

    /*What should happen every time the projectile moves (including the movement)*/
    protected override void InternalMovement(Vector2 movementDirection)
    {
        //first, move the sword
        mover.MoveActor(movementDirection);

        //Then calculate how far it has gone
        var distFromHome = Mathf.Abs(
            Vector3.Distance(
                startingPoint, 
                this.gameObject.transform.position
            )
        );

        //if it has gone too far, it should stop moving
        if(distFromHome >= travelDistance)
        {
            mover.MoveActor(Vector2.zero);
            this.moveFunction = null;
        }
    }

    //What happens when the projectile actually hits something
    protected override void OnTriggerEnter2D(Collider2D collided)
    {
        //if it's the player, make them pick up the sword
        if(collided.gameObject.CompareTag("Player"))
        {
            wrapper?.ReEquipSword();
        }
        //if not, just do the regular hit
        else
        {
            base.OnTriggerEnter2D(collided);
        }

    }
}

