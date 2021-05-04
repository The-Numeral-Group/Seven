using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(ActorMovement))]
public class PrideWaveProjectile : BasicProjectile
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

    [Header("Wave Knockback")]
    [Tooltip("Should this projectil ALWAYS drag the target in the oppisite direction of the" + 
        " prokectile.")]   
    public bool alwaysDragAwayFromCenter = false;

    [Tooltip("The direction targets will be dragged in when hit")]
    public Vector2 dragBackDirection = Vector2.left;

    [Tooltip("How intense the drag-force will be.")]
    public float dragBackIntensity = 2f;

    [Tooltip("How many seconds the drag should last.")]
    public float dragBackDuration = 2f;

    /*[Tooltip("Whether or not the wave should stop existing if it hits something.")]
    public bool destroyOnHit = true;*/

    //The function that will be called when the wave starts moving.
    //private Action<Vector2> moveFunction = null;

    //The direction the wave will be going;
    //private Vector2 launchDirection = Vector2.zero;

    //The wave's starting point (for determining how far it's gone)
    private Vector2 origin;

    //The wave's actor movement, which does its actual movement
    //private ActorMovement mover;

    //METHODS--------------------------------------------------------------------------------------
    // Start is called the first frame this object is active
    protected override void Start()
    {
        /*Preset the wave object's width so it scales correctly. We need to redefined the whole
        vector because it's a property and parts of properties can't be modified directly.*/
        this.gameObject.transform.localScale = new Vector3(
            minWaveWidth,
            this.gameObject.transform.localScale.y,
            this.gameObject.transform.localScale.z
        );

        //Get the ActorMovement for moving later
        this.mover = this.gameObject.GetComponent<ActorMovement>();
    }

    // fixedUpdate is called once per in-game tick (currently set to 60 ticks a second)
    /*void FixedUpdate()
    {
        //if there is a move function, invoke it with launch direction
        moveFunction?.Invoke(launchDirection);
    }*/

    /*What should happen every time the wave moves (including the movement)*/
    protected override void InternalMovement(Vector2 movementDirection)
    {
        //first, move the wave
        mover.MoveActor(movementDirection);

        //If the wave has hit something and we want that to destroy the wave, destroy the wave
        /*if(destroyOnHit && this.hitAlreadyLanded)
        {
            Destroy(this.gameObject);
        }*/

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
        ///DEBUG
        Debug.Log("PrideWaveProjectile: hit " + collided.gameObject.name);
        ///DEBUG

        //we just want the normal hitbox method
        var health = collided.gameObject.GetComponent<ActorHealth>();

        if(health)
        {
            health.takeDamage(damage);
        }

        //if the target can be dragged...
        ActorMovement enemyMove = null;
        if(collided.gameObject.TryGetComponent(out enemyMove))
        {
            //drag them back as well
            StartCoroutine(DragBack(enemyMove));
        }

        if(destroyOnHit)
        {
            Destroy(this.gameObject);
        }
    }

    /*Starts the wave!*/
    public override void Launch(Vector2 targetPoint, LAUNCH_MODE mode = LAUNCH_MODE.POINT)
    {
        //set origin (for later)
        origin = this.gameObject.transform.position;

        base.Launch(targetPoint, mode);

        //hard rotate the projectile
        this.gameObject.transform.up = launchDirection;

        //start the self-destruct timer
        StartCoroutine(durationTimer(maxWaveTime));
    }

    IEnumerator DragBack(ActorMovement mover)
    {
        //timer variable
        float clock = 0f;

        Vector2 dragAway;
        /*If desired, override the dragBackDirection with the direction from
        the user to the targe for an "Away" type of knock*/
        if(alwaysDragAwayFromCenter)
        {
            dragAway = (
                mover.gameObject.transform.position - this.gameObject.transform.position
            ).normalized;
        }
        else
        {
            dragAway = dragBackDirection;
        }

        //while there is still time remaining, drag one step and then yield
        while(clock < dragBackDuration)
        {
            mover.DragActor(dragAway * dragBackIntensity);
            yield return null;
            clock += Time.deltaTime;
        }
    }
}
