using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptedDataMovement : ActorMovement
{
    public int damage = 1;
    IEnumerator MovementLockRoutine;
    protected override void Awake()
    {
        base.Awake();
        speed = 0;
        MovementLockRoutine = LockActorMovement(100f);
    }

    public override void DragActor(Vector2 direction)
    {
        StopCoroutine(MovementLockRoutine);
        MovementLockRoutine = LockActorMovement(100f);
        StartCoroutine(MovementLockRoutine);
        base.DragActor(direction);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
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
            enemyHealth.takeDamage(damage);
        }
        Destroy(this.gameObject);
    }
    void OnTriggerStay2D(Collider2D collider)
    {
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
            enemyHealth.takeDamage(damage);
        }
        Destroy(this.gameObject);
    } 
}
