using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(ActorMovement))]
public class PrideWaveProjectile : BasicHitbox
{
    //FIELDS---------------------------------------------------------------------------------------
    [Header("Wave Size")]
    [Tooltip("Starting width of the shockwave.")]
    public float minWaveWidth = 1.0f;

    [Tooltip("Maximum width of the shockwave.")]
    public float maxWaveWidth = 20.0f;

    [Tooltip("How much the wave should grow per fixed update.")]
    public float waveGrowth = 1.0f;

    [Header("Wave Duration")]
    [Tooltip("The maximum distance this wave should travel before it stops.")]
    public float maxWaveDistance = 200.0f;

    [Tooltip("The maximum amount of time the wave should travel for.")]
    public float maxWaveTime = 10.0f;

    [Tooltip("Whether or not the wave should stop existing if it hits something.")]
    public bool destroyOnHit = true;

    //The function that will be called when the wave starts moving.
    private Action<Vector2> moveFunction = null;

    //The direction the wave will be going;
    private Vector2 launchDirection = Vector2.zero;

    //The wave's starting point (for determining how far it's gone)
    private Vector2 origin;

    //The wave's actor movement, which does its actual movement
    private ActorMovement mover;

    //METHODS--------------------------------------------------------------------------------------
    // Start is called the first frame this object is active
    void Start()
    {
        /*Preset the wave object's width so it scales correctly. We need to redefined the whole
        vector because it's a property and parts of properties can't be modified directly.*/
        this.gameObject.transform.localScale = new Vector3(
            minWaveWidth,
            this.gameObject.transform.localScale.y,
            this.gameObject.transform.localScale.z
        );

        //Get the ActorMovement for moving later
        mover = this.gameObject.GetComponent<ActorMovement>();
    }

    // fixedUpdate is called once per in-game tick (currently set to 60 ticks a second)
    void FixedUpdate()
    {
        //if there is a move function, invoke it with launch direction
        moveFunction?.Invoke(launchDirection);
    }

    /*What should happen every time the wave moves (including the movement)*/
    void InternalMovement(Vector2 movementDirection)
    {
        //first, move the wave
        mover.MoveActor(movementDirection);

        //If the wave has hit something and we want that to destroy the wave, destroy the wave
        if(destroyOnHit && this.hitAlreadyLanded)
        {
            Destroy(this.gameObject);
        }

        //calculate how far the wave has gone
        var traveledDistance = 
            Mathf.Abs(Vector2.Distance(this.gameObject.transform.position, origin));

        //if the wave has gone too far, destroy the wave
        if(traveledDistance >= maxWaveDistance)
        {
            Destroy(this.gameObject);
        }

        //if the wave still exists, gradually increase it (since it's supposed to act like a cone)
        var currentWaveWidth = this.gameObject.transform.localScale.x;
        if(currentWaveWidth < maxWaveWidth)
        {
            this.gameObject.transform.localScale = new Vector3(
            currentWaveWidth + waveGrowth,
            this.gameObject.transform.localScale.y,
            this.gameObject.transform.localScale.z
        );
        }
        
    }

    /*Simple coroutine timer for tracking the expiration time of the wave*/
    IEnumerator durationTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(this.gameObject);
    }

    protected override void OnTriggerEnter2D(Collider2D collided)
    {
        base.OnTriggerEnter2D(collided);
        if(destroyOnHit)
        {
            Destroy(this.gameObject);
        }
    }

    /*Starts the wave!*/
    public void Launch(Vector2 targetPoint)
    {
        //set origin (for later)
        origin = this.gameObject.transform.position;

        //set travel direction
        launchDirection = (targetPoint - origin).normalized;

        //rotate towards the thing
        this.gameObject.transform.rotation = 
            Rotations2D.LookRotation2D(targetPoint, Vector2.up);

        //S H M O O V E
        moveFunction = new Action<Vector2>(InternalMovement);

        //start the self-destruct timer
        StartCoroutine(durationTimer(maxWaveTime));
    }
}
