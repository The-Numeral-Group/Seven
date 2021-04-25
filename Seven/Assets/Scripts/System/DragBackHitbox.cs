using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragBackHitbox : WeaponHitbox
{
    public bool alwaysDragAwayFromUser = false;

    public Vector2 dragBackDirection = Vector2.left;

    public float dragBackIntensity = 2f;

    public float dragBackDuration = 2f;

    //OnTrigger function
    protected override void OnTriggerEnter2D(Collider2D collider)
    {

        /*I moved the wp getcomponent into the collider function. I realize this is inefficient but
        For bosses, since the parent object is swapped and the gameobjects are set to false in right
        away, there was an issue where the getcomponent was unable to find the weaponAbility when it
        was called during the start function*/

        this.wp = this.gameObject.GetComponentInParent(typeof(WeaponAbility)) as WeaponAbility;

        if (!this.wp)
        {
            Debug.LogWarning("Error: This WeaponHitbox is not the grandchild of an object with a WeaponAbility Script");
            return;
        }
        if (this.wp.hitConnected) //Check to see
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
            this.wp.hitConnected = true;
            enemyHealth.takeDamage(this.damage);
        }
        
        //if the enemy had an actor movement script...
        Component enemyMove = null;
        if(collider.gameObject.TryGetComponent(typeof(ActorMovement), out enemyMove))
        {
            //drag them back as well
            StartCoroutine(DragBack((enemyMove as ActorMovement)));
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collider)
    {
        if (!this.wp)
        {
            Debug.LogWarning("Error: This WeaponHitbox is not the grandchild of an object with a WeaponAbility Script");
            return;
        }
        if (this.wp.hitConnected) //Check to see
        {
            return;
        }
        Debug.Log("WeaponHitbox: collided with " + collider.gameObject.name);
        //try to get the enemy's health object
        var enemyHealth = collider.gameObject.GetComponent<ActorHealth>();

        //or a weakpoint if there's no regular health
        if (enemyHealth == null) { collider.gameObject.GetComponent<ActorWeakPoint>(); }

        {
            this.wp.hitConnected = true;
            enemyHealth.takeDamage(this.damage);
        }

        //if the enemy had an actor movement script...
        Component enemyMove = null;
        if(collider.gameObject.TryGetComponent(typeof(ActorMovement), out enemyMove))
        {
            //drag them back as well
            StartCoroutine(DragBack((enemyMove as ActorMovement)));
        }
    }

    IEnumerator DragBack(ActorMovement mover)
    {
        Debug.Log($"DragBackHitbox: dragging back {mover.gameObject.name}");
        float clock = 0f;

        Vector2 dragAway;
        /*If desired, override the dragBackDirection with the direction from
        the user to the targe for an "Away" type of knock*/
        if(alwaysDragAwayFromUser)
        {
            dragAway = (
                mover.gameObject.transform.position - this.wp.getUserTransform().position
            ).normalized;
        }
        else
        {
            dragAway = dragBackDirection;
        }

        while(clock < dragBackDuration)
        {
            mover.DragActor(dragBackDirection * dragBackIntensity);
            yield return null;
        }
    }
}
