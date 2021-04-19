using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple version of weapon hitbox.
public class Hitbox : MonoBehaviour
{
    public int damage = 1;
    public string targetTag = "Player";
    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        var enemyHealth = collider.gameObject.GetComponent<ActorHealth>();

        //or a weakpoint if there's no regular health
        if (enemyHealth == null) { collider.gameObject.GetComponent<ActorWeakPoint>(); }

        //if the enemy can take damage (if it has an ActorHealth component),
        //hurt them. Do nothing if they can't take damage.
        if (enemyHealth != null)
        {
            enemyHealth.takeDamage(this.damage);
        }
    }
}
