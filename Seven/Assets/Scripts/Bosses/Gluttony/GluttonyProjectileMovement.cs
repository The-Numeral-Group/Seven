using UnityEngine;
using System.Collections;

//This is a class for the gluttony projectile.
//It inherits from actor movement.
//Initiating its movement is meant to be called by other actors.
public class GluttonyProjectileMovement : ActorMovement
{
    //How long thiss projectile will last for. Must be greated thatn 0.
    public float projectileDuration = 20f;
    public int damage = 1;

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
    IEnumerator StopProjectile(float stopDelay)
    {
        
        yield return new WaitForSeconds(stopDelay);
        base.DragActor(Vector2.zero);
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(this.projectileDuration);
        Destroy(this.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag != "Player")
        {
            return;
        }
        else
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
                Destroy(this.gameObject);
            }
        }
    }   
}
