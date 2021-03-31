using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Doc: https://docs.google.com/document/d/1xz8Qk18jLRqImFmptbPEp0lnhTbAPQ-0BEOft3blKzM/edit
public class GhostKnightSlashCollision : MonoBehaviour
{
    public int damage = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //void OnCollisionEnter2D(Collision2D collider)
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
