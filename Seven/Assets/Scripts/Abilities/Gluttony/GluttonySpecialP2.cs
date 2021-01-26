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
    //The actor the user will target. Only requires their transform.position
    Actor targetActor;
    //Variable in charge of the direction the user will charge in
    Vector2 direction;
    //reference to camera for shake
    BaseCamera cam;
    //reference to first lockmovement
    IEnumerator stopLock;
    IEnumerator track;
    IEnumerator charge;

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

        var camObjects = FindObjectsOfType<BaseCamera>();
        if (camObjects.Length > 0)
        {
            cam = camObjects[0];
        }
        else
        {
            Debug.LogWarning("GluttonySpecialP2: does not have access to a camera that can shake");
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
        track = TrackTarget(args[0]);
        charge = Charge(args[0]);
        stopLock = args[0].myMovement.LockActorMovement(chargeDelay + 30f);
        StartCoroutine(stopLock);
        StartCoroutine(track);
        StartCoroutine(charge);
        return 0;
    }

    IEnumerator TrackTarget(Actor user)
    {
        while (true && targetActor)
        {
            direction = targetActor.transform.position - user.gameObject.transform.position;
            yield return new WaitForFixedUpdate();
        }
    }
    
    IEnumerator Charge(Actor user)
    {
        yield return new WaitForSeconds(chargeDelay);
        StopCoroutine(track);
        user.myMovement.DragActor(direction.normalized * user.myMovement.speed * specialSpeedModifier);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!isFinished)
        {
            cam.Shake(2.0f, 0.2f); 
            //StopAllCoroutines(); cannot dfo stop all coroutines cause it will stop the cooldown coroutine.
            StopCoroutine(stopLock);
            StopCoroutine(charge);
            StopCoroutine(track);
            this.gameObject.GetComponent<Actor>().myMovement.DragActor(Vector2.zero);
            isFinished = true;
            StartCoroutine(this.gameObject.GetComponent<Actor>().myMovement.LockActorMovement(0f));
        }
    }
}
