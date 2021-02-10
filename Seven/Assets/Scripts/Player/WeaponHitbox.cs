using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    //The damage for this hitbox. Should get set outside of the class.
    public int damage { get; set; }
    WeaponAbility wp;

    
    //OnTrigger function
 
    void OnTriggerEnter2D(Collider2D collider)
    {
        /*I moved the wp getcomponent into the collider function. I realize this is inefficient but
        For bosses, since the parent object is swapped and the gameobjects are set to false in right
        away, there was an issue where the getcomponent was unable to find the weaponAbility when it
        was called during the start function*/

        wp = this.gameObject.GetComponentInParent(typeof(WeaponAbility)) as WeaponAbility;
        this.damage = 1;

        if (!wp)
        {
            Debug.LogWarning("Error: This WeaponHitbox is not the grandchild of an object with a WeaponAbility Script");
            return;
        }
        if (wp.hitConnected) //Check to see
        {
            return;
        }
        //try to get the enemy's health object
        var enemyHealth = collider.gameObject.GetComponent<ActorHealth>();

        //or a weakpoint if there's no regular health
        if (enemyHealth == null) { collider.gameObject.GetComponent<ActorWeakPoint>(); }

        //if the enemy can take damage (if it has an ActorHealth component),
        //hurt them. Do nothing if they can't take damage.
        if (enemyHealth != null)
        {
            wp.hitConnected = true;
            enemyHealth.takeDamage(this.damage);
        }
    }

    public void PrintDamage()
    {
        Debug.Log(damage);
    }
}
