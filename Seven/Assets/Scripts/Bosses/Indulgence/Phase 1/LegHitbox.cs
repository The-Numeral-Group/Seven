using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegHitbox : Hitbox
{
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        var enemyHealth = collider.gameObject.GetComponent<ActorHealth>();

        //or a weakpoint if there's no regular health
        if (enemyHealth == null) { collider.gameObject.GetComponent<ActorWeakPoint>(); }

        //if the enemy can take damage (if it has an ActorHealth component),
        //hurt them. Do nothing if they can't take damage.
        if (enemyHealth != null)
        {
            //https://docs.unity3d.com/ScriptReference/Component.SendMessageUpwards.html
            SendMessageUpwards("PlayLegHit", SendMessageOptions.DontRequireReceiver);
            enemyHealth.takeDamage(this.damage);
        }
    }
}
