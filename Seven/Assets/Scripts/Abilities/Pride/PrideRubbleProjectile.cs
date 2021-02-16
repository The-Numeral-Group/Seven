using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(ActorMovement))]
public class PrideRubbleProjectile : MonoBehaviour
{
    //FIELDS---------------------------------------------------------------------------------------
    [Header("Slow Intensity")]
    [Tooltip("How slowed the target should be when they get hit.")]
    public float slowIntensity = 5.0f;

    [Tooltip("How long the target should be slowed for (in seconds).")]
    public float slowTime = 1.0f;

    [Header("Other Values")]
    [Tooltip("How much damage the rubble does")]
    public float damageValue = 3.0f;

    [Tooltip("Whether or not the rubble should stop existing if it hits something.")]
    public bool destroyOnHit = true;

    //The function that will be called when the rubble starts moving.
    private Action<Vector2> moveFunction = null;

    //The direction the rubble will be going;
    private Vector2 launchDirection = Vector2.zero;

    //The rubble's actor movement, which does its actual movement
    private ActorMovement mover;

    //The slow effect being applied to the player;
    private SimpleSlow slowEffect;

    //METHODS--------------------------------------------------------------------------------------
    // Awake is called every time the object is activated
    void Awake()
    {
        slowEffect = new SimpleSlow(slowIntensity);
    }
    // Start is called the first frame this object is active
    void Start()
    {
        //Get the ActorMovement for moving later
        mover = this.gameObject.GetComponent<ActorMovement>();
    }

    // fixedUpdate is called once per in-game tick (currently set to 60 ticks a second)
    void FixedUpdate()
    {
        //if there is a move function, invoke it with launch direction
        moveFunction?.Invoke(launchDirection);
    }

    /*What happens when things enter this object's collider*/
    void OnCollisionEnter2D(Collision2D collidedObject)
    {   
        //if the collided thing has an actorHealth, damage it
        collidedObject.gameObject.GetComponent<ActorHealth>()?.takeDamage(damageValue);

        //if the collided thing has an ActorEffectHandler, slow it
        //the below two lines are 1 program line, just indented for space
        collidedObject.gameObject.GetComponent<ActorEffectHandler>()
            ?.AddTimedEffect(slowEffect, slowTime);

        //finally, destroy the projectile
        Destroy(this.gameObject);
    }

    /*What should happen every time the rubble moves (including the movement)*/
    void InternalMovement(Vector2 movementDirection)
    {
        //first, move the rubble
        mover.MoveActor(movementDirection);
    }

    /*Starts the rubble!*/
    public void Launch(Vector2 targetPoint)
    {
        /*An explicit cast is added here, even though Vector3 implicitly converts
        to Vector2, to remove the ambiguity between subtracting the this.gameObject's position as
        a Vector3 or a Vector2*/
        launchDirection = (targetPoint - (Vector2)this.gameObject.transform.position).normalized;
        //S H M O O V E
        moveFunction = new Action<Vector2>(InternalMovement);
    }
}
