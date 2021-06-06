﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class just wraps up the real projectile ability to launch it however many times
public class ApathySludgeMortar : ActorAbilityFunction<GameObject, int>
{
    [Tooltip("The numer of projectile pairs this ability should launch")]
    public int pairCount = 3;

    [Header("Projectile Settings")]
    
    [Tooltip("The prefab of the Projectile.")]
    public GameObject projectile;

    [Tooltip("The prefab of the Marker.")]
    public GameObject marker;

    //the target will come from Invoke

    [Tooltip("How long the markers should follow before they stop moving.")]
    public float initialWaitTime = 3f;

    [Tooltip("How long it should take for a projectile to reach its target.")]
    public float shotFlightTime = 2f;

    [Header("Marker Settings")]
    [Tooltip("How far ahead of the target the leading markers should be.")]
    public float leadMarkerDistance = 5f;

    [Tooltip("How long a marker should wait to grab someone before it disappears")]
    public float waitDuration = 15f;

    [Tooltip("How long a marker should hold a target in place for")]
    public float grabDuration = 4f;

    //the single-shot version of the ability
    private ApathySludgeSingle single;
    
    // Start is called before the first frame update
    void Start()
    {
        single = this.gameObject.AddComponent<ApathySludgeSingle>();
        single.Init(this);
    }

    /*Activates the ability with no arguments. In this case, it will default the target to
    the player*/
    public override void Invoke(ref Actor user)
    {
        this.user = user;
        if(usable)
        {
            isFinished = false;
            InternInvoke(GameObject.FindWithTag("Player"));
        }
        
    }

    /*Same as the above method, but with a provided vector position*/
    public override void Invoke(ref Actor user, params object[] args)
    {
        this.user = user;
        //by default, Invoke just does InternInvoke with the provided arguments
        //it's also just implicitly convert the args and give it to InternInvoke
        if(usable)
        {
            isFinished = false;

            //make sure the argument is a gameObject of some sort
            if(args[0] is GameObject)
            {
                InternInvoke((args[0] as GameObject));
            }
            else if(args[0] is MonoBehaviour)
            {
                InternInvoke((args[0] as MonoBehaviour).gameObject);
            }
            else
            {
                Debug.LogError("apathyFireLaunch: firewall launched with improper target," + 
                    " must be GameObject or Monobehaviour");
            }
        }
    }

    //Activates an ActorAbility. Just wraps a coroutine.
    protected override int InternInvoke(params GameObject[] args)
    {
        usable = false;
        StartCoroutine(SludgeInvokation(args[0]));
        return 1;
    }

    IEnumerator SludgeInvokation(GameObject target)
    {
        //uhhh do it X many times idk
        for(int i = pairCount; i > 0 ; --i)
        {
            yield return new WaitUntil( () => single.getUsable() );
            Debug.Log($"ApathySludegeMortar: attack cycle {i}");
            single.target = target.transform;
            single.Invoke(ref user, target);
            yield return new WaitUntil( () => single.getIsFinished() );
        }

        isFinished = true;
        StartCoroutine(coolDown(cooldownPeriod));
    }
}

internal class ApathySludgeSingle : ProjectileAbility
{
    //All of thes variables carry the same meaning as they do in ApathySludgeMortar
    public Transform target { get; set; }

    private GameObject marker;

    private float leadMarkerDistance;

    private float initialWaitTime;

    private float shotFlightTime;

    private ApathySludgeMortar wrap;

    //recieves relevant initialization data
    public void Init(ApathySludgeMortar wrap)
    {
        this.projectile = wrap.projectile;
        this.marker = wrap.marker;
        this.leadMarkerDistance = wrap.leadMarkerDistance;
        this.initialWaitTime = wrap.initialWaitTime;
        this.shotFlightTime = wrap.shotFlightTime;

        this.wrap = wrap;
    }

    /*Launch the projectile. The anticipated argument is the gameObject being shot at. The 
    gameObject may-or-may-not have an ActorHealth component.*/
    protected override int InternInvoke(params Vector2[] args)
    {
        //deactivate projObj, we actually don't need it yet
        projObj.SetActive(false);

        //Start the long-term dup shooting
        StartCoroutine(ProjectileBehaviour());

        return 0;
    }

    /*The actual behaviour of the projectile sequence. Uses Coroutines for the literal timing of
    shots and markers.*/
    IEnumerator ProjectileBehaviour()
    {
        //Step -1: Start the animation
        //Going from Idle to prethrow to throwstance
        var animDelegate = this.user.myAnimationHandler.TryFlaggedSetTrigger("apathy_throw");

        //Step 0: check to see if there is a FaceAnchor for the target
        var targetFace = target.Find("FaceAnchor");

        //Step 1: Make a marker object where the player is standing
        //Instantiate Projectile makes this kinda easy
        var markObjA = this.InstantiateProjectile(marker, target, Vector2.zero);

        //Step 2: Actually make two
        //This one leads in front of the player though
        var markObjB = this.InstantiateProjectile(marker, target, target.up * leadMarkerDistance);

        //Step 3: Give them both marker components
        //and Step 4: Tell them to follow the player in their current relative positions
        markObjA.AddComponent<ApathySludgeMarker>().Follow(target, 0f, initialWaitTime);

        //markObjB will follow based off of faceAnchor if one exists
        markObjB.AddComponent<ApathySludgeMarker>()
            .Follow(target, leadMarkerDistance, initialWaitTime + shotFlightTime, targetFace);
        
        //Step 5: wait some seconds
        yield return new WaitForSeconds(initialWaitTime);
        //Step 5.5: and for the animation to finish
        //this should wait until the animator exits the prethrow state
        yield return new WaitWhile(animDelegate);

        //Step 6: Calcualte how fast the projectile needs to go to arrive in shotFlightTime seconds
        //d = rt: speed is distance to marker divided by shotFlightTime
        //Mathf.Abs because we want absolute speed (the projectile will handle direction)
        var newSpeed = 
            Mathf.Abs(Vector3.Distance(markObjA.transform.position, projObj.transform.position)) 
                / shotFlightTime;
        projObj.GetComponent<ActorMovement>().speed = newSpeed;

        //Step: 6.25 animate the next part of the throw
        //Going from throwstance to throw to idle
        animDelegate = this.user.myAnimationHandler.TryFlaggedSetTrigger("apathy_throw");

        //Step 6.5: Actually launch the projectile at the directly under marker now
        projObj.SetActive(true);
        projObj.GetComponent<FilterProjectile>()
            .Launch(markObjA.transform.position, LAUNCH_MODE.POINT, target.gameObject);

        //Step 6.... uh... 61?: Make a throw sound
        user.mySoundManager.PlaySound("Sludge_Throw");

        //Step 6.75: wait for the previous anim to finish
        //this should wait until the animator exits the throw state
        yield return new WaitWhile(animDelegate);

        //Step 6.875: animate the next part of the throw
        //Going from Idle to prethrow to throwstance
        animDelegate = this.user.myAnimationHandler.TryFlaggedSetTrigger("apathy_throw");

        //Step 7: Wait a little bit again...
        yield return new WaitForSeconds(shotFlightTime);

        //Step 7.5: And for the animation to finish
        //this should wait until the animator exits the prethrow state
        yield return new WaitWhile(animDelegate);
        

        
        //We need to double check here in case proj obj was destroyed on impact
        if(projObj)
        {
            Destroy(projObj);
            //Step 8: Destroy the marker and the projectile, if they still exist
            //Destroy(markObjA);
            markObjA.GetComponent<ApathySludgeMarker>()
                .BecomeGrabber(wrap.waitDuration, wrap.grabDuration);
        }
        else
        {
            //also destroy the marker, because the projectile must have hit the player
            Destroy(markObjA);
        }

        //Step 9: Now we need a brand new proj obj
        projObj = InstantiateProjectile(projectile, user.faceAnchor, projectileScale);

        //Step 10: Gotta switch this thing's speed too
        //d = rt: speed is distance to marker divided by shotFlightTime
        //Mathf.Abs because we want absolute speed (the projectile will handle direction)
        newSpeed = 
            Mathf.Abs(Vector3.Distance(markObjB.transform.position, projObj.transform.position)) 
                / shotFlightTime;
        projObj.GetComponent<ActorMovement>().speed = newSpeed;

        //Step: 10.25 animate the next part of the throw
        //we don't need to wait for this animation to end
        this.user.myAnimationHandler.TrySetTrigger("apathy_throw");

        //Step 10.5: And we're gonna launch it too
        projObj.GetComponent<FilterProjectile>()
            .Launch(markObjB.transform.position, LAUNCH_MODE.POINT, target.gameObject);

        //Step 10.75?: Make a throw sound
        user.mySoundManager.PlaySound("Sludge_Throw");

        //Step 11: Wait again...
        yield return new WaitForSeconds(shotFlightTime);

        
        //We need to double check here in case proj obj was destroyed on impact
        if(projObj)
        {
            Destroy(projObj);
            //Step 12: Destroy the marker and the projectile, if they still exist
            markObjB.GetComponent<ApathySludgeMarker>()
                .BecomeGrabber(wrap.waitDuration, wrap.grabDuration);
        }
        else
        {
            Destroy(markObjB);
        }

        //Step 13: Cooldown
        StartCoroutine(coolDown(cooldownPeriod));

        //Step 14: And now we're done! Whew!
        isFinished = true;
    }
}

internal class ApathySludgeMarker : MonoBehaviour
{
    private bool grabbing = false;

    private Transform markerTarget = null;

    private ActorMovement grabTarget = null;

    //This is a simple wrapper for the coroutine
    public void Follow(Transform target, float offset, float duration, Transform faceAnchor = null)
    {
        markerTarget = target;
        StartCoroutine(InternFollow(target, offset, duration, faceAnchor));
    }

    /*The following period. Will immediately snap this marker to the target's position,
    modified by the offset, as long as duration holds. Because there are NO phyiscal
    interations with these markers, it's okay to snap their positions directly.*/
    IEnumerator InternFollow(Transform target, float offset, 
        float duration, Transform faceAnchor)
    {
        float timer = 0f;

        //Dowhile will make the marker move before time is checked, so it always gets once
        //chance to update its position
        do
        {
            Vector3 directionToTarget;
            //if the target has a faceAnchor, judge the direction they're facing from that
            if(faceAnchor)
            {
                Vector3 pos = target.position;
                directionToTarget = (faceAnchor.position - pos).normalized;
                directionToTarget *= offset;
            }
            else
            {
                directionToTarget = target.up * offset;
            }
            this.gameObject.transform.position = target.position + directionToTarget;

            yield return null;

            timer += Time.deltaTime;
        }
        while(timer < duration);
    }

    //Turns a marker into a grabber so it can grab things
    public void BecomeGrabber(float waitDuration, float grabDuration)
    {
        StartCoroutine(Grab(waitDuration, grabDuration));
    }

    IEnumerator Grab(float waitDuration, float grabDuration)
    {
        this.gameObject.GetComponent<Animator>().SetTrigger("go");
        grabbing = true;
        //for the duration of the wait...
        for(float clock = 0f; clock < waitDuration; clock += Time.deltaTime)
        {
            //if something has been grabbed...
            if(grabTarget != null)
            {
                //lock them in place and wait it out
                grabTarget.StartCoroutine(grabTarget.LockActorMovement(grabDuration));
                if(grabTarget.TryGetComponent(out PlayerAbilityInitiator pai))
                {
                    pai.canDodge = false;
                }
                
                for(float grabClock = 0f; grabClock < grabDuration; grabClock += Time.deltaTime)
                {
                    //calculate the direction from the target to this object's center
                    Vector2 centerDir = 
                        ((this.gameObject.transform.position + new Vector3(0f, 1f, 0f))
                            - grabTarget.gameObject.transform.position).normalized;

                    //drag the target that way
                    grabTarget.DragActor(centerDir * 1.5f);
                    yield return null;
                }
                if(pai != null)
                {
                    pai.canDodge = true;
                }
                ConcludeMarker();
                yield break;
            }

            //if not, just keep ticking
            yield return null;
        }

        ConcludeMarker();
    }

    void OnTriggerEnter2D(Collider2D collided)
    {
        if(grabbing && collided.gameObject == markerTarget.gameObject)
        {
            if(!markerTarget.gameObject.TryGetComponent(out ActorMovement mov))
            {
                Debug.LogWarning($"ApathySludgeMortar: target {collided.gameObject.name} cannot" +  
                    " be grabbed.");
            }
            else
            {
                grabTarget = mov;
                grabbing = false;
            }
        }
    }

    void ConcludeMarker()
    {
        Destroy(this.gameObject);
    }
}