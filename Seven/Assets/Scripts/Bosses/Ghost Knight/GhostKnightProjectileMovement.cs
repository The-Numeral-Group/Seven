using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CURRENTLY NOT USING THIS SCRIPT
//This is a class for the ghost knight projectile.
//It inherits from actor movement.
//Initiating its movement is meant to be called by other actors.
public class GhostKnightProjectileMovement : ActorMovement
{
    //How long thiss projectile will last for. Must be greated thatn 0.
    public float projectileDuration = 5f;
    
    //Calls base actormovement start then starts a coroutine to destroy itself after a duration set by projectileDuraction.
    //Will also lock the movement of the projectile so movementdirection in ActorMovement has no bearing on it.
    protected override void Start()
    {
        base.Start();
        this.speed = 1f;
        if (this.projectileDuration < 0f)
        {
            Debug.Log("GhostKnightProjectile: Error duration cannot be less than 0");
            this.projectileDuration = 0f;
        }
        StartCoroutine(DestroySelf());
        StartCoroutine(LockActorMovement(this.projectileDuration));
    }

    public IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(this.projectileDuration);
        Destroy(this.gameObject);
    }
}
