using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDamage : MonoBehaviour
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("The amount of damage the ActorHealth should take.")]
    public float damage = 0f;

    [Tooltip("The ActorHealth to damage.")]
    public ActorHealth health;

    //METHODS--------------------------------------------------------------------------------------

    //Makes the specified health take damage. Duh.
    public void DamageHealth()
    {
        Debug.Log("Autodamaging Dude");
        health.takeDamage(damage);
    }
}
