using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHitbox : MonoBehaviour
{
    //The damage for this hitbox. Should get set outside of the class.
    public int damage { get; set; }
    protected PlayerWeaponAbility wp;

    //OnTrigger function
    void OnTriggerEnter2D(Collider2D collider)
    {
        //this.damage = 1;
        wp = this.gameObject.GetComponentInParent(typeof(PlayerWeaponAbility)) as PlayerWeaponAbility;

        if (!wp)
        {
            Debug.LogWarning("PlayerWeaponHitbox: Cannot find PlayerWeaponAbility script from parent objects!");
            return;
        }
        if (wp.hitConnected) // If the hitbox has already hit an enemy, return
        {
            return;
        }

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
            wp.hitConnected = true;
            enemyHealth.takeDamage(this.damage);
        }
    }
}
