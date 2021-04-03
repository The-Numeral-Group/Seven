using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a class for the ghost knight projectile.
// It inherits from actor movement.
// Initiating its movement is meant to be called by other actors.
// Doc: https://docs.google.com/document/d/1IjiWRfhoDrXga_CNwXKgooaWsfhEg5MWACxjHEZWKSs/edit
public class GhostKnightProjectileMovement : ActorMovement
{

    private Actor player;

    public float projectileDuration = 5f;
    public int damage = 1;
    public float before_fadeAway = 4.5f;
    public float fadeAway_duration = 0.5f;

    private float projectileSpeed = 0.1f;

    ActorSoundManager soundManager;


    protected override void Start()
    {
        base.Start();

        var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];
        player = playerObject.GetComponent<Actor>();

        this.soundManager = this.gameObject.GetComponentInChildren<ActorSoundManager>();

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
        yield return new WaitForSeconds(this.before_fadeAway);

        SpriteRenderer projSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();

        float opacity = 1f;
        while (opacity > 0f)
        {
            opacity -= 0.1f;
            projSpriteRenderer.color = new Color(1f, 1f, 1f, opacity);
            yield return new WaitForSeconds(this.fadeAway_duration / 10);
        }
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
                    soundManager.PlaySoundAtClip("ProjectileHit");
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
