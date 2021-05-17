using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathSwordAttackCollision : MonoBehaviour
{
    public int damage;

    void OnTriggerEnter2D(Collider2D collider)
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
                this.gameObject.GetComponent<ActorSoundManager>().PlaySound("AttackHit");
                playerHealth.takeDamage(damage);
            }
        }
    }
}
