using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a class for the ghost knight projectile.
//It inherits from actor movement.
//Initiating its movement is meant to be called by other actors.
public class GhostKnightProjectileMovement : ActorMovement
{

    private Actor player;

    public int damage = 1;

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
    void OnCollisionEnter2D(Collision2D collider)
    {
        // Only collide with player
        if (collider.gameObject.tag == "Player")
        {
            var playerHealth = collider.gameObject.GetComponent<ActorHealth>();

            //or a weakpoint if there's no regular health
            if (playerHealth == null) { collider.gameObject.GetComponent<ActorWeakPoint>(); }

            //if the enemy can take damage (if it has an ActorHealth component),
            //hurt them. Do nothing if they can't take damage.
            if (playerHealth != null)
            {
                if (!playerHealth.vulnerable)
                {
                    return;
                }
                playerHealth.takeDamage(damage);
                Destroy(this.gameObject);
            }
        }
        else // If Collide with something else, then just pass through
        {
            Physics2D.IgnoreCollision(collider.collider, this.gameObject.GetComponent<CircleCollider2D>());
        }
    }
}
