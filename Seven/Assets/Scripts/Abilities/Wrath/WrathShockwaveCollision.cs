using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathShockwaveCollision : MonoBehaviour
{
    public int damage;

    public float duration;

    public int additionalDamage;
    public float delaySpeedMultiplier;

    private void Start()
    {
        additionalDamage = WrathP2Actor.abilityDamageAddition;
        delaySpeedMultiplier = WrathP2Actor.abilitySpeedMultiplier;
        StartCoroutine(DestroyObject());
    }

    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(duration / delaySpeedMultiplier);
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
                playerHealth.takeDamage(damage + additionalDamage);
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        // Only collide with player
        if (other.gameObject.tag == "Player")
        {
            var playerHealth = other.gameObject.GetComponent<ActorHealth>();

            //or a weakpoint if there's no regular health
            if (playerHealth == null) { other.gameObject.GetComponent<ActorWeakPoint>(); }

            //if the enemy can take damage (if it has an ActorHealth component),
            //hurt them. Do nothing if they can't take damage.
            if (playerHealth != null)
            {
                if (!playerHealth.vulnerable)
                {
                    return;
                }
                //this.gameObject.GetComponent<ActorSoundManager>().PlaySound("AttackHit");
                playerHealth.takeDamage(damage + additionalDamage);
            }
        }
    }
}
