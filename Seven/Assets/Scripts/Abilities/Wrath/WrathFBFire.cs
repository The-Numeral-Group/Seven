﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathFBFire : ActorMovement
{
    // How much damage the fireball will do
    public int damage;

    // How long the fireball will take to drop
    public float duration;

    public float delayCollide;

    private int additionalDamage;
    private float delaySpeedMultiplier;

    protected override void Start()
    {
        base.Start();

        additionalDamage = WrathP2Actor.abilityDamageAddition;
        delaySpeedMultiplier = WrathP2Actor.abilitySpeedMultiplier;

        if(delaySpeedMultiplier == 1.5)
        {
            delaySpeedMultiplier = 1.42f;
        }

        StartCoroutine(LockActorMovement(Mathf.Infinity));
        StartCoroutine(flyProjectile());
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private IEnumerator flyProjectile()
    {
        Vector2 parentPos = this.transform.parent.gameObject.transform.position;
        parentPos.y += 2.5f;
        float distance = Vector2.Distance(this.transform.position, parentPos);
        float flyingSpeed = distance / (this.duration / delaySpeedMultiplier);

        this.DragActor(new Vector2(0.0f, -1.0f) * flyingSpeed);

        yield return new WaitForSeconds(this.duration / delaySpeedMultiplier);

        this.DragActor(Vector2.zero);

        // Delay before turning on Collider
        //yield return new WaitForSeconds(this.delayCollide / delaySpeedMultiplier);

        // turn on collider
        this.GetComponent<PolygonCollider2D>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only collide with player
        if (collision.gameObject.tag == "Player")
        {
            var playerHealth = collision.gameObject.GetComponent<ActorHealth>();

            //or a weakpoint if there's no regular health
            if (playerHealth == null) { collision.gameObject.GetComponent<ActorWeakPoint>(); }

            //if the enemy can take damage (if it has an ActorHealth component),
            //hurt them. Do nothing if they can't take damage.
            if (playerHealth != null)
            {
                if (playerHealth.vulnerable)
                {
                    playerHealth.takeDamage(damage + additionalDamage);
                }
                Destroy(this.gameObject);
            }
        }
    }
}
