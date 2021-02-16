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

    ///DEBUG
    //[Tooltip("How long the knockback should last.")]
    //public float knockbackDuration;
    ///DEBUG

    [Tooltip("Whether knockbackDirection should be overriden by whichever direction is 'away'" + 
        " from the knockback source (that is, by a vector that points from this box to whoever" +
            " got hit).")]
    public bool forceOppositeKnockbackDirection = true;

    //METHODS--------------------------------------------------------------------------------------
    //OnTrigger function
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        /*I moved the wp getcomponent into the collider function. I realize this is inefficient but
        For bosses, since the parent object is swapped and the gameobjects are set to false in right
        away, there was an issue where the getcomponent was unable to find the weaponAbility when it
        was called during the start function*/
        wp = this.gameObject.GetComponentInParent(typeof(WeaponAbility)) as WeaponAbility;
        
        //DEBUG
        //OnCollisionEnter2D(collider.collider);
        if (!wp)
        {
            Debug.LogWarning("Error: This WeaponHitbox is not the grandchild of an object with a WeaponAbility Script");
            return;
        }
        if (wp.hitConnected) //Check to see
        {
            return;
        }
        //Debug.Log("WeaponHitbox: collided with " + collider.gameObject.name);
        //try to get the enemy's health object
        var enemyHealth = collider.gameObject.GetComponent<ActorHealth>();

        //or a weakpoint if there's no regular health
        if(enemyHealth == null){enemyHealth = collider.gameObject.GetComponent<ActorWeakPoint>();}

        //if the enemy can take damage (if it has an ActorHealth component),
        //hurt them. Do nothing if they can't take damage.
        if(enemyHealth != null){
            //Debug.Log("WeaponHitbox: Health was found on " + enemyHealth.gameObject.name);
            wp.hitConnected = true;
            enemyHealth.takeDamage(this.damage);
            ApplyKnockback(enemyHealth);
        }
        //DEBUG
    }

    protected override void OnCollisionEnter2D(Collision2D collider)
    {
        if (!wp)
        {
            Debug.LogWarning("Error: This WeaponHitbox is not the grandchild of an object with a WeaponAbility Script");
            return;
        }
        if (wp.hitConnected) //Check to see
        {
            return;
        }
        Debug.Log("WeaponHitbox: collided with " + collider.gameObject.name);
        //try to get the enemy's health object
        var enemyHealth = collider.gameObject.GetComponent<ActorHealth>();

        //or a weakpoint if there's no regular health
        if(enemyHealth == null){collider.gameObject.GetComponent<ActorWeakPoint>();}

        //if the enemy can take damage (if it has an ActorHealth component),
        //hurt them. Do nothing if they can't take damage.
        if(enemyHealth != null){
            wp.hitConnected = true;
            enemyHealth.takeDamage(this.damage);
            ApplyKnockback(enemyHealth);
        }
    }

    void ApplyKnockback(ActorHealth enemyHealth)
    {
        Rigidbody2D enemyRigid = enemyHealth.gameObject.GetComponent<Rigidbody2D>();
        if(!enemyRigid)
        {
            ///DEBUG
            Debug.Log("KnockbackWeaponHitbox: Aborting Knockback...");
            ///DEBUG
            return;
        }

        if(forceOppositeKnockbackDirection)
        {
            var boxPos = this.gameObject.transform.position;
            var enemyPos = enemyHealth.gameObject.transform.position;
            Vector2 antinormalVec = (enemyPos - boxPos).normalized;
            ///DEBUG
            Debug.Log("KnockbackWeaponHitbox: attempting to launch " + enemyRigid.gameObject.name + " with force of " + antinormalVec * knockbackIntensity);
            Debug.DrawRay(enemyPos, antinormalVec * knockbackIntensity, Color.green, 0.5f);
            
            enemyRigid.AddForce(antinormalVec * knockbackIntensity, ForceMode2D.Impulse);
            //var enemyMove = enemyHealth.gameObject.GetComponent<ActorMovement>();
            //enemyMove?.LockActorMovement(knockbackDuration);
            //enemyMove?.DragActor(antinormalVec * knockbackIntensity);
            ///DEBUG
        }
        else
        {
            //these next two lines are one line, just fyi
            //enemyRigid.AddForce(knockbackDirection * knockbackIntensity, ForceMode2D.Impulse);
        }
    }
}
