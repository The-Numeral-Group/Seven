using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHitbox : MonoBehaviour
{
    //The damage for this hitbox. Should get set outside of the class.
    public int damage { get; set; }

    //OnTrigger function
    void OnTriggerEnter2D(Collider2D collider)
    {
        this.damage = 1;

        //Debug.Log("WeaponHitbox: collided with " + collider.gameObject.name);
        //try to get the enemy's health object
        var enemyHealth = collider.gameObject.GetComponent<ActorHealth>();

        //or a weakpoint if there's no regular health
        if (enemyHealth == null) { enemyHealth = collider.gameObject.GetComponent<ActorWeakPoint>(); }

        //if the enemy can take damage (if it has an ActorHealth component),
        //hurt them. Do nothing if they can't take damage.
        if (enemyHealth != null)
        {
            if (!enemyHealth.vulnerable)
            {
                return;
            }
            enemyHealth.takeDamage(this.damage);
        }
    }
}
