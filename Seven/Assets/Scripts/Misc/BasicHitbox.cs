using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Dead-basic hitbox. It's a box that hits you. Activating it and
reseting it for a second attack is the user's problem*/
[RequireComponent(typeof(Collider2D))]
public class BasicHitbox : MonoBehaviour
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("How much damage this hitbox should cause.")]
    public float damage;

    [Tooltip("Whether or not this hitbox should be ignored due to already striking something.")]
    public bool hitAlreadyLanded = false;

    //METHODS--------------------------------------------------------------------------------------
    void OnTriggerEnter2D(Collider2D collided)
    {
        if(!hitAlreadyLanded)
        {
            var health = collided.gameObject.GetComponent<ActorHealth>();

            if(health)
            {
                health.takeDamage(damage);
                hitAlreadyLanded = true;
            }
        }
    }
}
