using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MarkerMode 
{
    BOUND_MARKER,
    EXACT_MARKER,
    FRONT_MARKER,
    NONE
}

enum DelayMode
{
    WINDUP,
    THROW
}

//This class just wraps up the real projectile ability to launch it however many times
public class ApathySludgeSnipe : ActorAbilityCoroutine<GameObject>
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("The numer of projectile pairs this ability should launch")]
    public int pairCount = 3;

    [Header("Prefab Settings")]
    
    [Tooltip("The prefab of the Projectile.")]
    public GameObject projectile;

    [Tooltip("The prefab of the Marker.")]
    public GameObject marker;

    //the target will come from Invoke
    [Header("Targeting Settings")]
    [Tooltip("How long the markers should follow before they stop moving and the user attacks" + 
        " if the target HASN't been grabbed.")]
    public float initialWaitTime = 3f;

    [Tooltip("How long the markers should follow before they stop moving and the user attacks" + 
        " if the target HAS been grabbed.")]
    public float postRevengeWaitTime = 3f;

    [Tooltip("How long it should take for a projectile to reach its target.")]
    public float shotFlightTime = 2f;

    [Tooltip("Marker Settings")]
    [Tooltip("How far ahead of the target the leading markers should be.")]
    public float leadMarkerDistance = 5f;

    [Tooltip("How long a marker should remain active before self-destructing.")]
    public float grabDuration = 45f;

    [Tooltip("How many times the target will need to mash the interact button to escape.")]
    public int mashCount = 10;

    //the target transform
    private Transform target;

    //the single-shot version of the ability
    private ApathySnipeSingle single;

    //the queue of markers to throw at
    private Queue<ApathySnipeMarker> throwQueue;
    
    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        single = this.gameObject.AddComponent<ApathySnipeSingle>();

        throwQueue = new Queue<ApathySnipeMarker>();
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
                Debug.LogError("ApathySludgeSnipe: launched with improper target," + 
                    " must be GameObject or Monobehaviour");
            }
        }
    }

    //The full logic of the ability. Once the throwQueue is empty, the ability is over
    protected override IEnumerator InternCoroutine(params GameObject[] args)
    {
        //Step -1: Initialize single
        target = args[0];
        single.Init(this);

        //for each needed throw...
        //notice pair count is doubled, because we need twice as many projectiles
        for(int i = 0; i < pairCount * 2; ++i)
        {
            /*Step -0.5: If this is an even invokation, remind single to rebuild its markers,
            since any previous ones should have been expended.*/
            if(i % 2 == 0)
            {
                single.RebuildMarkers();
            }

            /*Step 0: Resolve any markers that occured while this ability was not in use*/
            while(throwQueue.Count != 0)
            {
                //get the marker
                var marker = throwQueue.Dequeue();

                //bind it to the single-shot
                single.BindMarker(marker);

                //do the single-shot
                single.Invoke(DelayMode.THROW, MarkerMode.BOUND_MARKER);

                //wait for the single shot to end
                yield return new WaitUntil( () => single.getIsFinished() );
            }

            //Step 1: Resolve marker type
            //the player will no longer be grabbed at this point
            //so, just do the normal throw thing
            //The kind of marker that is locked to depends on which iteration we're on
            var normalMarker = (i % 2 == 0) ? MarkerMode.EXACT_MARKER : MarkerMode.FRONT_MARKER;

            //Step 2: Time the regular throw
            //start the initial wait clock
            for(float clock = 0f; clock < initialWaitTime; clock += Time.deltaTime)
            {
                //if someone's been grabbed...
                if(throwQueue.Count != 0)
                {
                    //get the marker
                    var marker = throwQueue.Dequeue();

                    //bind it to the single-shot
                    single.BindMarker(marker);

                    //do the single-shot
                    single.Invoke(DelayMode.THROW, MarkerMode.BOUND_MARKER);

                    //wait for the single shot to end
                    yield return new WaitUntil( () => single.getIsFinished() );

                    //enter the revenge stance
                    yield return RevengeStance(postRevengeWaitTime, normalMarker);

                    //exit the timer, since RevengeStance will do the rest of it
                    break;
                }
            }

            //Step 3: Do the regular throw
            single.Invoke(DelayMode.THROW, normalMarker);

            //wait until it's done
            yield return new WaitUntil( () => single.getIsFinished() );

            //now repeat!
        }

        //and now we're done!
    }

    //Handles interrupting a windup to attack the target if they get stuck
    IEnumerator RevengeStance(float revengeDelay, MarkerMode targetMarker)
    {
       //wind up apathy
        single.Invoke(DelayMode.WINDUP);
        yield return new WaitUntil( () => single.getIsFinished() );

        //wait out the clock...
        for(float clock = 0f; clock < revengeDelay; clock += Time.deltaTime)
        {
            //if someone's been grabbed...
            if(throwQueue.Count != 0)
            {
                //get the marker
                var marker = throwQueue.Dequeue();

                //bind it to the single-shot
                single.BindMarker(marker);

                //do the single-shot
                single.Invoke(DelayMode.THROW, MarkerMode.BOUND_MARKER);

                //wait for the single shot to end
                yield return new WaitUntil( () => single.getIsFinished() );

                //reset the clock
                //can this be recursive? Yes. Should it? Probably not.
                clock = 0f;
            }
        }

        //Once the clock runs out, just throw at the initial target
        single.Invoke(DelayMode.THROW, targetMarker);
        yield return new WaitUntil( () => single.getIsFinished() );
    }

    public void AlertToMarker(ApathySnipeMarker marker)
    {
        throwQueue.Enqueue(marker.gameObject);
    }
}

//The actual projectile launch, compartamentailized for readability
internal class ApathySnipeSingle : ProjectileAbility
{
    //FIELDS---------------------------------------------------------------------------------------
    //All of these variables carry the same meaning as they do in ApathySludgeSnipe
    public Transform target { get; set; }

    private GameObject marker;

    private float leadMarkerDistance;

    private float initialWaitTime;

    private float shotFlightTime;

    public float grabDuration;

    //the marker under the player's feet
    private GameObject directMarker;

    //the marker in front of the player
    private GameObject leaderMarker;

    //an abitrary "bound" marker
    private GameObject boundMarker;

    //whether or not this attack has already played the windup 
    private bool woundup = false;

    //METHODS--------------------------------------------------------------------------------------
    //recieves relevant initialization data
    public void Init(ApathySludgeMortar wrap)
    {
        this.projectile = wrap.projectile;
        this.marker = wrap.marker;
        this.leadMarkerDistance = wrap.leadMarkerDistance;
        this.shotFlightTime = wrap.shotFlightTime;
        this.grabDuration = wrap.grabDuration;

        this.launchMode = LAUNCH_MODE.POINT;

        RebuildMarkers();
    }

    //creates the two markers that the attack can use: one direct and one leader. They both start
    //following immediately
    RebuildMarkers()
    {
        //get the target's faceAnchor, if it exists
        var targetFace = target.Find("FaceAnchor");

        //create the markers
        directMarker = this.InstantiateProjectile(marker, target, Vector2.zero);
        leaderMarker = this.InstantiateProjectile(marker, target, target.up * leadMarkerDistance);

        //give them the needed components and set them to follow
        directMarker.AddComponent<ApathySnipeMarker>().Follow(target, 0f);
        leaderMarker.AddComponent<ApathySnipeMarker>()
            .Follow(target, leadMarkerDistance, targetFace);
    }

    //setter method for boundMarker, Just in case...
    BindMarker(GameObject marker)
    {
        marker.GetComponent<ApathySnipeMarker>().StopFollow(grabDuration);
        marker = boundMarker;
    }

    //Does nothing, to require that the user provides Delay and Marker Modes
    public override void Invoke(ref Actor user)
    {
        Debug.LogError("ApathySnipeClass: no non-argument invoker defined");   
    }

    //Sets up the ability and processes enumarator arguments to perform the correct functioality
    public override void Invoke(ref Actor user, params object[] args)
    {
        this.user = user;
        if(usable)
        {
            isFinished = false;

            /*static cast the desired enums. This isn't type safe, but this class should
            only ever be used by SludgeSnipe.*/
            DelayMode delay = args[0] as DelayMode;
            MarkerMode marker = args.Length >= 2 ? args[1] as MarkerMode : MarkerMode.NONE; 

            //If it's just a windup call...
            if(DelayMode)
            {
                //just play the windup animation
                StartCorotuine(Windup());
            }
            //If this is a full throw
            else
            {
                //make projectile
                projObj = this.InstantiateProjectile(projectile, user.faceAnchor, projectileScale);
            
                //and throw it!
                InternInvoke(PrepTarget(marker));
            
                StartCoroutine(coolDown(cooldownPeriod));
            }
        }
        
    }

    /*Launch the projectile. The anticipated argument is the gameObject being shot at. The 
    gameObject may-or-may-not have an ActorHealth component.*/
    protected override int InternInvoke(params Vector2[] args)
    {
        StartCoroutine(AnimatedProjectile(args[0]));

        return 0;
    }

    //the exact same as a normal InternInvoke, but with animation timings
    //normally this would be an ActorAbilityCoroutine, but I (Thomas) want to use
    //ProjectileAbility's methods
    IEnumerator AnimatedProjectile(Vector2 coords)
    {
        //play the windup animation, if needed
        if(!woundup)
        {
            yield return Windup();
        }

        /*//launch the projectile
        projObj.GetComponent<BasicProjectile>().Launch(args[0], launchMode);
        Actually, this needs to be a filter projectile
        Also, it will need a component for deciding whether or not to destroy an associated
        marker when it dies. However, they might be managed by this coroutine, and not
        by the projectile*/

        //play the throw animation
        yield return new WaitWhile(user.myAnimationHandler.TryFlaggedSetTrigger("apathy_throw"));

        //and now we're done
        isFinished = true;
    }

    //plays Apathy's Throw Windup animation, if present, and does nothing else
    IEnumerator Windup()
    {
        yield return new WaitWhile(user.myAnimationHandler.TryFlaggedSetTrigger("apathy_throw"));
        woundup = true;
    }

    //Determines which coordinates to give InternInvoke
    //Also stops the marker chosen, if needed
    Vector2 PrepTarget(MarkerMode marker)
    {
        GameObject outMarker;

        switch(marker)
        {
            case MarkerMode.BOUND_MARKER:
            {
                //the bound marker should already be stopped
                outMarker = boundMarker;
            }

            case MarkerMode.DIRECT_MARKER:
            {
                directMarker.GetComponent<ApathySnipeMarker>().StopFollow(grabDuration);
                outMarker = directMarker;
                break;
            }

            case MarkerMode.LEADER_MARKER:
            {
                leaderMarker.GetComponent<ApathySnipeMarker>().StopFollow(grabDuration);
                outMarker = leaderMarker;
                break;
            }

            default:
            {
                Debug.LogError("ApathySludgeSnipe: ApathySnipeSingle: invalid marker desired");
                return null;
                break;
            }
        }

        return outMarker.transform.position;
    }
}

//the logic for the marker, which handles following the player and when grabs should occur
/*Still needs to somehow pass the amount of grabs it should do, and the target, to the player
maybe make follow pass a wrap like how snipe does for single?*/
internal class ApathySnipeMarker : MonoBehaviour
{
    private bool targetIsGrabbed = false;

    private GameObject target;

    private ApathySludgeSnipe wrap;

    private IEnumerator grabTimer;

    private System.Action activity = null;

    void Update()
    {
        activity?.Invoke();
    }
    
    //Turns on following logic for the marker by placing said logic in the activity delegate
    public void Follow(Transform target, float offset, float grabDuration, Transform faceAnchor = null)
    {
        //turn off any grabbing timers
        StopCoroutine(grabTimer);

        this.target = target;

        //and turn on the follow logic
        activity = new System.Action( () => 
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
        });
    }

    //turns off the follow logic and starts the grab timer
    public void StopFollow(float grabDuration=0.0f)
    {
        //if there is a grab timer, then this marker has already stopped
        if(grabTimer != null)
        {
            return;
        }
        //turn off frame-updating
        activity = null;
        
        //start grabbing
        grabTimer = StartCoroutine(BecomeGrabber(grabDuration));
    }

    //turns a marker into a grabber, which will allow it to lock targets in place
    IEnumerator BecomeGrabber(float grabDuration)
    {
        //Mostly just an internal timer which self-destructs the icon once it's too old
        yield return new WaitForSeconds(grabDuration);
        
    }

    //Self-Destructs the object and does needed cleanup
    public void Cleanup()
    {
        //if the player is mashing...
        var mash = target.gameObject.GetComponent<ApathyMashChecker>();
        //and they're mashing on this marker...
        if(mash?.host == this)
        {
            //they can stop
            mash.Cleanup();
        }

        Destroy(this.gameObject);
    }
}

//Counts the player's interact presses to see how much they are mashing
//relys on the notion that the player can interact with itself
internal class ApathyMashChecker : Interactable
{
    //FIELDS---------------------------------------------------------------------------------------
    //the amount of times interact must be pressed before the target "passes" and is freed
    private int mashCount = 10;

    //METHODS--------------------------------------------------------------------------------------
    public void Init()
    {
        //haha idk yet?
        base.OnTriggerEnter2D(this.gameObject.GetComponent<Collider2D>());
    }

    public override void OnInteract()
    {
        mashCount -= 1;
        if(mashCount <= 0)
        {
            Cleanup();
        }
    }

    public void Cleanup()
    {
        //uncollide with the player
        base.OnTriggerExit2D(this.gameObject.GetComponent<Collider2D>());
    
        //and self-destruct
        Destroy(this);
    }
}