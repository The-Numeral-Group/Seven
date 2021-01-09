using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))] //I (Ram) do not know if it is correct to require the 2d collider.
public class WeaponHitbox : MonoBehaviour
{
    public int damage { get; set; }
    WeaponAbility wp;

    void Start()
    {
        //Assuming the Hierarchy of objects is Actor->weapon->weaponhitbox
        wp = this.transform.parent.transform.parent.gameObject.GetComponent<WeaponAbility>();
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        if (wp == null)
        {
            Debug.Log("Error: This WeaponHitbox is not the grandchild of an object with a WeaponAbility Script");
            return;
        }
        if (wp.hitConnected) //Check to see
        {
            return;
        }
        wp.hitConnected = true;
        //try to get the enemy's health object
        var enemyHealth = collider.gameObject.GetComponent<ActorHealth>();
        /*I (Ram) commented out the weakpoint code because I don't believe it is in the new repo.
        //or a weakpoint if there's no regular health
        if(enemyHealth == null){collider.gameObject.GetComponent<ActorWeakPoint>();}*/

        //if the enemy can take damage (if it has an ActorHealth component),
        //hurt them. Do nothing if they can't take damage.
        if(enemyHealth != null){
            enemyHealth.takeDamage(this.damage);
        }
    }
}
