using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a class for the ghost knight projectile.
//It inherits from actor movement.
//Initiating its movement is meant to be called by other actors.
public class GhostKnightProjectileMovement : ActorMovement
{

    private Actor player;

    public float projectileDuration = 5f;
    public int damage = 1;

    private float projectileSpeed = 0.1f;

    protected override void Start()
    {
        base.Start();

        var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];
        player = playerObject.GetComponent<Actor>();

        StartCoroutine(DestroySelf());
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

    protected override void InternalMoveActor()
    {
        //calculate a composite movement vector
        Vector2 moveComposite = this.movementDirection * projectileSpeed;

        //actually moving
        movementController.Move(moveComposite);

    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(this.projectileDuration);
        Destroy(this.gameObject);
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
                if (playerHealth.vulnerable)
                {
                    playerHealth.takeDamage(damage);
                }
                Destroy(this.gameObject);
            }
        }
        else // If Collide with something else, then just pass through
        {
            Physics2D.IgnoreCollision(collider.collider, this.gameObject.GetComponent<CircleCollider2D>());
        }
    }
}
