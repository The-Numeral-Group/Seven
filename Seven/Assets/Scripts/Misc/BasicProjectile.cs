using System;
using UnityEngine;

public enum LAUNCH_MODE
{
    POINT,
    DIRECTION
}

[RequireComponent(typeof(ActorMovement))]
[RequireComponent(typeof(Collider2D))]
public class BasicProjectile : MonoBehaviour
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("How much damage this hitbox should cause.")]
    public float damage;

    [Tooltip("Whether or not the projectile should stop existing if it hits something.")]
    public bool destroyOnHit = true;

    //The function that will be called when the rubble starts moving.
    protected Action<Vector2> moveFunction = null;

    //The direction the rubble will be going;
    protected Vector2 launchDirection = Vector2.zero;

    //The rubble's actor movement, which does its actual movement
    protected ActorMovement mover;

    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        //Get the ActorMovement for moving later
        mover = this.gameObject.GetComponent<ActorMovement>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        moveFunction?.Invoke(launchDirection);
    }

    //What happens when the projectile actually hits something
    protected virtual void OnTriggerEnter2D(Collider2D collided)
    {
        //we just want the normal hitbox method
        var health = collided.gameObject.GetComponent<ActorHealth>();

        if(health)
        {
            health.takeDamage(damage);
        }

        if(destroyOnHit)
        {
            Destroy(this.gameObject);
        }
    }

    /*What should happen every time the projectile moves (including the movement)*/
    protected virtual void InternalMovement(Vector2 movementDirection)
    {
        //first, move the rubble
        mover.MoveActor(movementDirection);
    }

    /*Starts the projectile!*/
    public virtual void Launch(Vector2 target, LAUNCH_MODE mode = LAUNCH_MODE.POINT)
    {
        /*An explicit cast is added here, even though Vector3 implicitly converts
        to Vector2, to remove the ambiguity between subtracting the this.gameObject's position as
        a Vector3 or a Vector2*/
        if(mode == LAUNCH_MODE.POINT)
        {
            launchDirection = (target - (Vector2)this.gameObject.transform.position).normalized;
        }
        else
        {
            launchDirection = target.normalized;
        }

        //S H M O O V E
        moveFunction = new Action<Vector2>(InternalMovement);
    }
}
