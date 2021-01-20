using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a class for the ghost knight projectile.
//It inherits from actor movement.
//Initiating its movement is meant to be called by other actors.
public class GhostKnightProjectileMovement : ActorMovement
{

    private Actor player;

    protected override void Start()
    {
        base.Start();

        var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];
        player = playerObject.GetComponent<Actor>();
    }
    protected override void FixedUpdate()
    {
        FollowPlayer();
        InternalMoveActor();
    }

    private void FollowPlayer()
    {
        var myPos = this.gameObject.transform.position;
        var playerPos = player.gameObject.transform.position;

        this.movementDirection = (playerPos - myPos).normalized;
    }
}
