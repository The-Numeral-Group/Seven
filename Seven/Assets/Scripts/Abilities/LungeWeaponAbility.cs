using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LungeWeaponAbility : WindupWeaponAbility
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("The boolean to flag the animator that the windup animation should loop.")]
    public string animFlag = "";

    [Tooltip("The trigger to activate the recovery animation, if any.")]
    public string recoveryTrigger = "";

    //the ways in which LungeWeaponAbility can calcuate its travelling
    public enum TRAVEL_MODE
    {
        DISTANCE,   //will judge duration of lunge by distance
        TIME        //will judge duration of lunge by time taken
    }

    [Tooltip("How the timing of the lunge should be calculated. DISTANCE will stop after the user" + 
        " has travelled lungeDistance meters. TIME will stop after the user travels for" + 
            " lungeDuration seconds. Both modes will use the same lungeSpeed, and both modes" + 
                " will stop if the user hits something.")]
    public TRAVEL_MODE mode = TRAVEL_MODE.DISTANCE;

    [Tooltip("How far the lunge will go in DISTANCE mode")]
    public float lungeDistance = 20f;

    [Tooltip("How long the lunge will last in TIME mode.")]
    public float lungeDuration = 2f;

    [Tooltip("How fast the user will move while lunging.")]
    public float lungeSpeed = 10f;

    [Tooltip("How long the user should be locked in place after the attack ends.")]
    public float endlagDuration = 2f;

    [Tooltip("The name of the sound to play when this attack hits something")]
    public string strikeSound;

    //the user's speed prior to the lunge (as to reset the user's speed post-lunge)
    private float origSpeed = 0f;

    //the time when the lunge was started, for timing calculations
    private float origTime = 0f;

    //the original position of the user, for distance calculations
    private Vector3 origPos = Vector3.zero;

    /*the direction in which the user is lunging*/
    private Vector3 moveDir = Vector3.zero;

    //the func use to calculate, execute, and judge the movement of the lunge
    private Func<bool> travelling;

    //METHODS--------------------------------------------------------------------------------------
    // Generates the Func used for the travelling logic
    protected override void Start()
    {
        //create the required travelling func based on public fields
        if(mode == TRAVEL_MODE.DISTANCE)
        {
            travelling = DistanceTravel(lungeDistance); 
        }
        else
        {
            travelling = TimeTravel(lungeDuration);
        }
        //do the superclass starter
        base.Start();
    }

    /*The same sort of delay as WindupWeaponAbility, but with additional delays
    for travel and for an "endlag" period.*/
    protected override IEnumerator delayedAttack(params Actor[] args)
    {
        //flag the user's animator that the windup animation needs to loop
        //Starting that animation is handled in base.InternInvoke.
        this.user.myAnimationHandler.TrySetBool(animFlag, true);

        //calculate the direction to lunge in
        moveDir = (user.faceAnchor.position - user.gameObject.transform.position).normalized;
        //save the user's current position
        origPos = this.user.gameObject.transform.position;
        //save the current time
        origTime = Time.time;

        //clean the old attack instance
        this.hitConnected = false;
        StopCoroutine(sheathe);

        //play the windup sound
        user.mySoundManager?.PlaySound(this.windupSound);

        //wait for the windup
        //if the screen should shake, start the minor shake
        if(shouldShake){ cameraFunc.Shake(windupDelay, windShake); }
        yield return new WaitForSeconds(windupDelay);

        user.mySoundManager?.StopSound(this.windupSound);
        user.mySoundManager?.PlaySound(this.attackSound);
        
        //now that the windup is over, tell the animator to stop looping
        this.user.myAnimationHandler.TrySetBool(animFlag, false);

        //then do the rest of the attack as normal
        sheathe = SheatheWeapon();
        weaponObject.SetActive(true);
        SpawnWeapon(args);

        //start the screen shake here, since we can't override SheathWeapon()
        if(shouldShake){ cameraFunc.Shake(duration, attackShake); }

        //save the user's speed and change it to the lunge speed
        origSpeed = user.myMovement.speed;
        user.myMovement.speed = lungeSpeed;

        //wait until the travel has completed
        yield return new WaitWhile(travelling);

        user.myMovement.speed = origSpeed;

        //play the attack sound
        
        //sheathe the weapon
        yield return sheathe;

        //user.mySoundManager?.StopSound(this.attackSound);

        //enter the recovery animation
        var recoverAnimFinished = 
            this.user.myAnimationHandler.TryFlaggedSetTrigger(this.recoveryTrigger);

        //experience the endlag
        yield return user.myMovement.LockActorMovement(endlagDuration);

        //make sure the recovery animation has also ended
        yield return new WaitUntil(recoverAnimFinished);
    }

    /*factory method. Whenever the func is evaluated, it will check if the user has
    hit something or if the user has gone too far. Is so, it will return false. If
    not, it will take a step forward in the travelling direction and return true.*/
    Func<bool> DistanceTravel(float distance)
    {
        return new Func<bool>( () =>
        {
            var fullDist = Mathf.Abs(
                Vector3.Distance(
                    origPos, 
                    this.user.gameObject.transform.position
                )
            );

            ///DEBUG
            Debug.Log($"LungeWeaponAbility: hitconnected is {this.hitConnected}");
            ///DEBUG

            if(!this.hitConnected && fullDist < lungeDistance)
            {
                this.user.myMovement.MoveActor(moveDir);

                return true;
            }

            return false;
        } );
    }

    /*factory method. Whenever the func is evaluated, it will check if the user has
    hit something or if the user has taken too long. Is so, it will return false. If
    not, it will take a step forward in the travelling direction and return true.
    Tragically, this does not perform any time travel outside of the forward time travel
    needed to execute a program.*/
    Func<bool> TimeTravel(float time)
    {
        return new Func<bool>( () => 
        {
            var fullTime = Time.time - origTime;

            ///DEBUG
            Debug.Log($"LungeWeaponAbility: hitconnected is {this.hitConnected}");
            ///DEBUG

            if(!this.hitConnected && fullTime < lungeDuration)
            {
                this.user.myMovement.MoveActor(moveDir);

                return true;
            }

            if(this.hitConnected)
            {
                user.mySoundManager?.PlaySound(this.strikeSound);
            }

            return false;
        } );
    }

}
