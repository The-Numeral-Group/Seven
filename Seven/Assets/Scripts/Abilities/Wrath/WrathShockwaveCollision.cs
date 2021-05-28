using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathShockwaveCollision : MonoBehaviour
{
    public int damage;

    public float duration;

    private void Start()
    {
        StartCoroutine(DestroyObject());
    }

    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(duration);
        Destroy(this.gameObject);
    }

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
                //this.gameObject.GetComponent<ActorSoundManager>().PlaySound("AttackHit");
                playerHealth.takeDamage(damage);
            }
        }
    }
}
