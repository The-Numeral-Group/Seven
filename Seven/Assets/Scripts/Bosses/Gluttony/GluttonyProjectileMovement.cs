using UnityEngine;
using System.Collections;

//This is a class for the gluttony projectile.
//It inherits from actor movement.
//Initiating its movement is meant to be called by other actors.
public class GluttonyProjectileMovement : ActorMovement
{
    //How long thiss projectile will last for. Must be greated thatn 0.
    public float projectileDuration = 20f;

    //Calls base actormovement start then starts a coroutine to destroy itself after a duration set by projectileDuraction.
    //Will also lock the movement of the projectile so movementdirection in ActorMovement has no bearing on it.
    protected override void Start()
    {
        base.Start();
        this.speed = 1f;
        if (this.projectileDuration < 0f)
        {
            Debug.Log("GluttonyProjectile: Error duration cannot be less than 0");
            this.projectileDuration = 0f;
        }
        StartCoroutine(DestroySelf());
        StartCoroutine(LockActorMovement(this.projectileDuration));
    }

    //Similar to base class dragActor but calls a coroutine which will stop the projectile.
    //Will also lock the projectiles d
    public override void DragActor(Vector2 direction)
    {
        base.DragActor(direction * speed);
        float stopDelay = Random.Range(1.5f, 2f);
        StartCoroutine(StopProjectile(stopDelay));
    }

    //Given a direction
    public IEnumerator StopProjectile(float stopDelay)
    {
        
        yield return new WaitForSeconds(stopDelay);
        base.DragActor(Vector2.zero);
    }

    public IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(this.projectileDuration);
        Destroy(this.gameObject);
    }   
}
