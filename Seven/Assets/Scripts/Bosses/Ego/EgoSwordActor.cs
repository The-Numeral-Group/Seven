﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EgoSwordActor : Actor
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("How many seconds this sword should fly for")]
    public float flightDuration = 3f;

    [Tooltip("How long this sword should wait between laser strikes")]
    public float landDuration = 3f;

    [Tooltip("How many times this laser should attack before disappearing")]
    public int attackCount = 4;

    [Tooltip("How much the sword should rotate per movement step (higher numbers will make" + 
        " swords turn faster.")]
    [Range(0f, 1f)]
    public float turnMax = 0.05f;

    [Tooltip("How much damage this sword should deal to things that walk into it.")]
    public float collisionDamage = 1f;

    [Tooltip("The ActorAbility this sword should attack the player with after it lands.")]
    public ActorAbility swordAbility;

    private enum SwordState
    {
        FLYING,
        LANDED
    }

    private SwordState state = SwordState.FLYING;

    //the gameObject this sword will try to steer towards
    private GameObject target;

    //the delegate to invoke every frame for movement
    private System.Action moveDelegate;

    //writable this reference
    private Actor wthis;

    //METHODS--------------------------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();

        this.mySoundManager.PlaySound("sword_appear");
    }
    // Invoked every simulation tick (every 60th of a second)    
    void FixedUpdate()
    {
        moveDelegate?.Invoke();
    }

    // Invoked when something else collides with this object
    void OnCollisionEnter2D(Collision2D col)
    {
        //if the collided thing is the target...
        if(col.gameObject == target)
        {
            //try to hurt it
            //can't do send message because ActorHealth has 1 argument, even though it's optional
            target.GetComponent<ActorHealth>()?.takeDamage(collisionDamage);
            //SendMessage("takeDamage", collisionDamage, SendMessageOptions.DontRequireReceiver);

            //if the sword was flying, it should self destruct at this point
            if(state == SwordState.FLYING)
            {
                Cleanup();
            }
        }
    }

    //activates the sword and gives it a target and an ability
    public void Launch(GameObject target, ActorAbility newAbility=null)
    {
        this.target = target;

        /*Be wary: if you reassign the sword's ability and that ability instance is destroyed,
        the sword will crash the game. It is HIGHLY recommended that the newAbility be an
        instance that is directly attached to the sword.*/
        if(newAbility != null)
        {
            swordAbility = newAbility;
        }

        //set writable self reference
        wthis = this;

        StartCoroutine(SwordBehaviour());
    }

    public void DelayFollowLaunch(
        GameObject target, Vector3 offset, float duration, ActorAbility newAbility=null)
    {
        StartCoroutine(DelayFollow(target, offset, duration, newAbility));
    }

    IEnumerator DelayFollow(
        GameObject target, Vector3 offset, float duration, ActorAbility newAbility=null)
    {
        float clock = 0f;

        while(clock <= duration)
        {
            this.gameObject.transform.position = target.transform.position + offset;

            yield return null;

            clock += Time.deltaTime;
        }

        Launch(target, newAbility);
    }

    /*The sword's behaviour. Swords are defined to fly towards the target for a certain amount of 
    time. If they hit their target, they self-destruct. If they don't hit their target in that
    time, they will stop moving and shoot Ego Lasers at them for a certain amount of time.*/
    IEnumerator SwordBehaviour()
    {
        //Step 1: set the movement delegate to InternalMovement
        moveDelegate = new System.Action( () => {InternalMove();} );

        //Step 2: wait through the flight time
        yield return new WaitForSeconds(flightDuration);

        //Step 3: Land. Disable the movement vector
        moveDelegate = null;
        //this line prevents the sword from being pushed by other things
        this.gameObject.GetComponent<Rigidbody2D>().constraints 
            = RigidbodyConstraints2D.FreezeAll;
        state = SwordState.LANDED;

        //Step 3.5: Switch to the landed animation. And, no, I am not going to write
        //a custom AAH just for this one thing
        this.myAnimationHandler.Animator.SetBool("egosword_landed", true);
        this.mySoundManager.PlaySound("sword_land");

        //Step 4: Shoot the lasers
        for(int i = 0; i < attackCount; ++i)
        {
            //Step 4.1: Wait between the lasers
            yield return new WaitForSeconds(landDuration);

            //Step 4.2: shoot the actual lasers
            //And yes I should be using an AAI but I didn't want to bother for just the one thing
            swordAbility.Invoke(ref wthis, target);

            //Step 4.3: Wait for the laser to finish
            yield return new WaitUntil( () => swordAbility.getIsFinished() );
        }

        //Step 5: Sword's done. Destroy it.
        Cleanup();
    }

    //The sword's movement logic for when it is flying
    void InternalMove()
    {
        /*first, rotate towards the player a certain number of degrees
        Do do that, the sword will just rotate to a certain lerp between its current
        direction and the direction it needs to be going*/
        var flightDirection = (target.transform.position - this.transform.position).normalized;

        this.gameObject.transform.up = 
            Vector3.Lerp(this.gameObject.transform.up, flightDirection, turnMax); 

        //Then, move in the direction this gameObject is currently moving
        //var flightDirection = (target.transform.position - this.transform.position).normalized;
        this.myMovement.MoveActor(this.gameObject.transform.up);
    }

    //cleans up the sword, which includes destroying it
    void Cleanup()
    {
        Destroy(this.gameObject);
    }
}
