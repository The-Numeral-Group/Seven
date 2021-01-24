using UnityEngine;
using System.Collections;

//This is a class for the gluttony projectile.
//It inherits from actor movement.
//Initiating its movement is meant to be called by other actors.
public class GluttonyProjectileMovement : ActorMovement
{
    //How long this projectile will last for. Must be greated thatn 0.
    [Tooltip("How long the projectile lasts for before destroying itself.")]
    public float projectileDuration = 20f;
    //Damage the projectile can collide with a player.
    [Tooltip("How much damage the projectiles will do to the player.")]
    public int damage = 1;
    //The range the time the projectile will travel for.
    [Tooltip("The range the time the projectile will travel for. Values ideally >= 0 and not equal.")]
    public Vector2 stopDelayRange = new Vector2(1.5f, 2.0f);
    //Should the projectile become a static object after it finishes travelling
    [Tooltip("Should the projectile become a static object after it travels?")]
    public bool makeStatic = false;

    protected override void Awake()
    {
        base.Awake();
        if (projectileDuration < 0f)
        {
            Debug.Log("GluttonyProjectile: Error duration cannot be less than 0");
            projectileDuration = 0f;
        }
        if (stopDelayRange.x < 0 || stopDelayRange.y < 0 || stopDelayRange.x >= stopDelayRange.y)
        {
            Debug.Log("GluttonyProjectile: x must be < y and both must be >= to 0.");
            stopDelayRange.x = 0f;
            stopDelayRange.y = 0.1f;
        }
    }

    /*Calls base actormovement start then starts a coroutine to destroy 
    itself after a duration set by projectileDuraction.
    Will also lock the movement of the projectile so 
    movementdirection in ActorMovement has no bearing on it.*/
    protected override void Start()
    {
        base.Start();
        StartCoroutine(DestroySelf());
        StartCoroutine(LockActorMovement(projectileDuration));
    }

    //Similar to base class dragActor but calls a coroutine which will stop the projectile.
    //Will also lock the projectiles d
    public override void DragActor(Vector2 direction)
    {
        base.DragActor(direction * speed);
        float stopDelay = Random.Range(stopDelayRange.x, stopDelayRange.y);
        StartCoroutine(StopProjectile(stopDelay));
    }

    //Given a direction
    IEnumerator StopProjectile(float stopDelay)
    {
        
        yield return new WaitForSeconds(stopDelay);
        base.DragActor(Vector2.zero);
        if (makeStatic)
        {
            //https://answers.unity.com/questions/1301204/how-to-change-rigidbody2d-body-type-or-change-whet.html
            this.rigidbody.bodyType = RigidbodyType2D.Static;
        }
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(projectileDuration);
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
