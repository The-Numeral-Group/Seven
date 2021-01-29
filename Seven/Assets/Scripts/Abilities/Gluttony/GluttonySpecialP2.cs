using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GluttonySpecialP2 : ActorAbilityFunction<Actor, int>
{
    //The value usued to adjust the charging speed. 
    [Tooltip("Speed modifier for the charge. Charge speed = ActorBaseSpeed * specialSpeedModifier")]
    public float specialSpeedModifier = 10;
    //The delay between when the user acquires its target and charges.
    [Tooltip("How much time the user will take to track its target.")]
    public float chargeDelay = 1f;
    //The length in seconds of the charge
    [Tooltip("How long the charge will last for.")]
    public float chargeDuration = 4f;
    //The actor the user will target. Only requires their transform.position
    Actor targetActor;
    //Variable in charge of the direction the user will charge in
    Vector2 direction;

    //Initialize member variables
    void Awake()
    {
        direction = Vector2.zero;
    }

    //initialize monobehaviour fields
    void Start()
    {
        var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];
        if(playerObject == null)
        {
            Debug.LogWarning("GluttonySpecialp2: Gluttony can't find the player!");
        }
        else
        {
            targetActor = playerObject.GetComponent<Actor>();
        }
    }

    /*Invoke passes a reference of the user to the InternInvoke method
    The ability will only be engaged if the cooldown period has finished, and that
    the ability is not already in use by the actor*/
    public override void Invoke(ref Actor user)
    {
        if(this.usable && targetActor && this.isFinished)
        {
            this.isFinished = false;
            StartCoroutine(coolDown(cooldownPeriod));
            InternInvoke(user);
        }
    }
    protected override int InternInvoke(params Actor[] args)
    {
        IEnumerator track = TrackTarget(args[0]);
        StartCoroutine(args[0].myMovement.LockActorMovement(chargeDelay + chargeDuration));
        StartCoroutine(track);
        StartCoroutine(Charge(args[0], track));
        return 0;
    }

    IEnumerator TrackTarget(Actor user)
    {
        while (true && targetActor)
        {
            direction = targetActor.transform.position - user.transform.position;
            yield return new WaitForFixedUpdate();
        }
    }
    
    IEnumerator Charge(Actor user, IEnumerator stopTrack)
    {
        yield return new WaitForSeconds(chargeDelay);
        StopCoroutine(stopTrack);
        user.myMovement.DragActor(direction.normalized * user.myMovement.speed * specialSpeedModifier);
        yield return new WaitForSeconds(chargeDuration);
        this.isFinished = true;
    }
}
