using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class IndulgenceP1ProjMovement : ActorMovement
{
    //How long this projectile will last for. Must be greated thatn 0.
    [Tooltip("How long the projectile lasts for before destroying itself.")]
    public float projectileDuration = 20f;
    //Damage the projectile can collide with a player.
    [Tooltip("How much damage the projectiles will do to the player.")]
    public int damage = 1;
    //The range the time the projectile will travel for.
    [Tooltip("The range the time the projectile will travel for. Values ideally >= 0 and not equal.")]
    public Vector2 stopDelayRange = new Vector2(1.0f, 5.0f);
    //Should the projectile become a static object after it finishes travelling
    [Tooltip("Should the projectile become a static object after it travels or a trigger hazard?")]
    public bool makeStatic = false;
    public Collider2D projCollider;
    public Rigidbody2D rb;
    public SpriteRenderer sp;
    public string targetTag = "Player";

    protected override void Awake()
    {
        speed = 0f;
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
        StartCoroutine(BlinkRed());
    }

    //Similar to base class dragActor but calls a coroutine which will stop the projectile.
    //Will also lock the projectiles d
    public override void DragActor(Vector2 direction)
    {
        StartCoroutine(DestroySelf());
        StartCoroutine(LockActorMovement(projectileDuration));
        base.DragActor(direction);
        float stopDelay = Random.Range(stopDelayRange.x, stopDelayRange.y);
        StartCoroutine(StopProjectile(stopDelay));
    }

    //Given a direction
    IEnumerator StopProjectile(float stopDelay)
    {   
        yield return new WaitForSeconds(stopDelay);
        base.DragActor(Vector2.zero);
        if (!makeStatic)
        {
            projCollider.isTrigger = true;
        }
        else
        {
            //rb.bodyType = RigidbodyType2D.Static;
            damage = 1;
        }
        this.gameObject.layer = LayerMask.NameToLayer("Boss Projectile");
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(projectileDuration);
        Destroy(this.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag != targetTag)
        {
            return;
        }
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
            if (!makeStatic)
            {
                Destroy(this.gameObject);
            }
        }
    }

    void OnCollisionStay2D(Collision2D collider)
    {
        if (collider.gameObject.tag != targetTag)
        {
            return;
        }
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
            if (!makeStatic)
            {
                Destroy(this.gameObject);
            }
        }
    }

    IEnumerator BlinkRed()
    {
        Color defaultColor = sp.color;
        Color modifiedColor = sp.color;
        while(true)
        {
            modifiedColor.r = 1;
            modifiedColor.g = 0;
            modifiedColor.b = 0;
            sp.color = modifiedColor;
            yield return new WaitForSeconds(0.3f);
            modifiedColor.r = defaultColor.r;
            modifiedColor.g = defaultColor.g;
            modifiedColor.b = defaultColor.b;
            sp.color = modifiedColor;yield return new WaitForSeconds(0.3f);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag != targetTag)
        {
            return;
        }
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
    }   
}
