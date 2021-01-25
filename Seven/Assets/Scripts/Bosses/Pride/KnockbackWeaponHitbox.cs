using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackWeaponHitbox : WeaponHitbox
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("Which direction the knockback will be applied, relative to this hitbox.")]
    public Vector2 knockbackDirection;

    [Tooltip("How intense the knockback force should be.")]
    public float knockbackIntensity;

    [Tooltip("Whether knockbackDirection should be overriden by whichever direction is 'away'" + 
        " from the knockback source (that is, by a vector that points from this box to whoever" +
            " got hit).")]
    public bool forceOppositeKnockbackDirection = true;

    //METHODS--------------------------------------------------------------------------------------

    /*Called when this collides with something. Used for hitbox detection*/
    protected override void OnCollisionEnter2D(Collision2D collider)
    {
        if (!this.wp)
        {
            Debug.Log("Error: This WeaponHitbox is not the grandchild of an" + 
                " object with a WeaponAbility Script");
            return;
        }
        if (this.wp.hitConnected) //Check to see
        {
            return;
        }
        //try to get the enemy's health object
        var enemyHealth = collider.gameObject.GetComponent<ActorHealth>();

        //or a weakpoint if there's no regular health
        if(enemyHealth == null){collider.gameObject.GetComponent<ActorWeakPoint>();}

        //if the enemy can take damage (if it has an ActorHealth component),
        //hurt them. Do nothing if they can't take damage.
        if(enemyHealth != null){
            if (!enemyHealth.vulnerable)
            {
                return;
            }
            this.wp.hitConnected = true;
            enemyHealth.takeDamage(this.damage);

            //This is the only change from the vanilla implementation from WeaponHitbox
            
            if(forceOppositeKnockbackDirection)
            {
                var boxPos = this.gameObject.transform.position;
                var enemyPos = enemyHealth.gameObject.transform.position;
                var antinormalVec = (boxPos - enemyPos).normalized;
                enemyHealth.gameObject.GetComponent<Rigidbody2D>()
                    ?.AddForce(antinormalVec * knockbackIntensity, ForceMode2D.Impulse);
            }
            else
            {
                //these next two lines are one line, just fyi
                enemyHealth.gameObject.GetComponent<Rigidbody2D>()
                    ?.AddForce(knockbackDirection * knockbackIntensity, ForceMode2D.Impulse);
            }
            
        }
    }


}
