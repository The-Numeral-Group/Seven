using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Gluttonys crush ability
currently the camera shake seems to be procuding weird efx.
Camera shake has been commented out for the time being*/
public class GluttonyCrush : ActorAbilityFunction<Actor, int>
{
    //Shadow object should for now have a cript that will tie it's movement with Gluttony.
    [Tooltip("Prefab that will be instantiated to the target actors position.")]
    public GameObject toInstantiateShadowSprite;
    //the offet being used to spawn the shadowSprite on the target
    [Tooltip("The offset for the position of the shadow sprite relative to the target actor.")]
    public Vector2 distanceFromActor = new Vector2(0, -6);
    //The sin object which will be spawned after the crush.
    [Tooltip("The sin prefab to be instantiated.")]
    public GameObject sin;
    //How high gluttony jumps into the air;
    [Tooltip("How high the user will move off screen.")]
    float verticalOffset = 120f;
    //The speed at which the user will fall.
    [Tooltip("How fast the user will fall. verticalOffset / fallSpeed = second(s) user takes to fall.")]
    public float fallSpeed = 120;
    //The speed at which the user will be moved offscreen
    [Tooltip("How fast the user will jump offscreen.")]
    public float jumpSpeed = 10;
    //How long the user will wait before tracking target
    [Tooltip("How long the user will wait before tracking the target.")]
    public float jumpDuration = 1f;
    //How long the user will track the target
    [Tooltip("How long the user will track the target.")]
    public float trackDuration = 2f;
    //How long the user will be stationary after the crush
    [Tooltip("How long the user will be stationary after crush.")]
    public float crushDelay = 1f;
    //The sum of the above three durations. Used to lock the movement of the user.
    float totalAbilityDuration;
    //The actor this move is meant to target
    Actor targetActor;
    //pointer to the main camera shake component to initialize camera efx
    BaseCamera cam;

    //Initialize member variables
    void Awake()
    {
        //Compount all the coroutine delays to get the total duration of the ability.
        this.totalAbilityDuration = this.jumpDuration + this.trackDuration + this.crushDelay;
    }

    //Initializing monobehavior fields
    void Start()
    {
        var camObjects = FindObjectsOfType<BaseCamera>();
        var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];
        if(playerObject == null)
        {
            Debug.LogWarning("GluttonyCrush: Gluttony can't find the player!");
        }
        else
        {
            targetActor = playerObject.GetComponent<Actor>();
        }
        if (camObjects.Length > 0)
        {
            cam = camObjects[0];
        }
        else
        {
            Debug.LogWarning("Gluttony Crush: does not have access to a camera that can shake");
        }
    }

    /*Similar to the Invoke for ActorAbilityFunction but explicitly checks if the move has been
    finished. Passes the users actor component to interninvoke*/
    public override void Invoke(ref Actor user)//(ref Actor user)
    {
        if(this.usable && targetActor && this.isFinished)
        {
            this.isFinished = false;
            StartCoroutine(coolDown(cooldownPeriod));
            InternInvoke(user);
        }
    }

    /*Used as a way to start the three-part coroutine that makes up the ability.
    Initiates the first portion which is JumpUp.
    Passes the direction to jump towards.*/
    protected override int InternInvoke(params Actor[] args)
    {
        Vector2 destination = new Vector2(targetActor.transform.position.x, 
                                            targetActor.transform.position.y + verticalOffset);
        Vector2 direction = (destination - new Vector2(args[0].transform.position.x, 
                                                        args[0].transform.position.y)).normalized;
        direction = direction * jumpSpeed;
        IEnumerator initialMovementLock = args[0].myMovement.LockActorMovement(totalAbilityDuration);
        StartCoroutine(initialMovementLock);
        StartCoroutine(JumpUp(args[0], direction, initialMovementLock));
        return 0;
    }

    /*Part 1 of the 3 steps to perform crush.
    Moves the ability user to in the defined direction.
    Instatiates an object to represent its final destination.
    Calls both Part 2 and 3 of this process.*/
    IEnumerator JumpUp(Actor user, Vector2 direction, IEnumerator initialMovementLock)
    {
        var actorColliders = user.gameObject.GetComponents<Collider2D>();
        foreach(var actorCollider in actorColliders)
        {
            actorCollider.enabled = false;
        }
        user.myMovement.DragActor(direction);
        yield return new WaitForSeconds(jumpDuration);
        if (targetActor)
        {
            GameObject shadowSprite = Instantiate(toInstantiateShadowSprite, 
                                        new Vector3(targetActor.transform.position.x + distanceFromActor.x, 
                                        targetActor.transform.position.y + distanceFromActor.y, 
                                        targetActor.transform.position.z), Quaternion.identity);
            shadowSprite.transform.parent = targetActor.transform;
            IEnumerator trackTarget = TrackTargetWithShadow(user, shadowSprite);
            IEnumerator crush = Crush(user, shadowSprite, trackTarget, initialMovementLock);
            StartCoroutine(trackTarget);
            StartCoroutine(crush);
        }
    }

    /*Part 2 of the 3 steps to perform crush.
    Tracks the targets movement by having the ability user mimic its' 
    targets movementdirection and speed.
    Loops infinitely, but is stopped by Part 3.*/
    IEnumerator TrackTargetWithShadow(Actor user, GameObject shadowSprite)
    {
        user.transform.position = new Vector3(targetActor.transform.position.x, 
                                                targetActor.transform.position.y + 
                                                verticalOffset,user.transform.position.z);
        
        shadowSprite.transform.parent = user.gameObject.transform;
        while(true && targetActor)
        {
            yield return new WaitForFixedUpdate();
            Vector2 direction = new Vector2(targetActor.transform.position.x - shadowSprite.transform.position.x,
                targetActor.transform.position.y - shadowSprite.transform.position.y + distanceFromActor.y);
            user.myMovement.DragActor(direction.normalized * targetActor.myMovement.speed);
        }
    }

    /*Part 3 of the 3 steps to perform crush.
    Stops part 2 after a specified duration as passed.
    Will then move the ability user towards the shadowSprites position.
    Starts the aftermath function which performs the abilities efx.*/
    IEnumerator Crush(Actor user, GameObject shadowSprite, IEnumerator stopTrack, IEnumerator initialLockMovement)
    {
        yield return new WaitForSeconds(trackDuration);
        StopCoroutine(stopTrack);
        user.myMovement.DragActor(Vector2.zero);
        shadowSprite.transform.parent = null;
        yield return new WaitForSeconds(crushDelay);
        Vector2 direction = (shadowSprite.transform.position - user.gameObject.transform.position).normalized;
        float timeToArrival = Vector2.Distance(shadowSprite.transform.position, 
                                                user.gameObject.transform.position) / fallSpeed;
        user.myMovement.DragActor(direction * fallSpeed);

        //I (Ram) do not know what will happen if a StopCoroutine is used on a coroutine that has already stopped.
        StopCoroutine(initialLockMovement);
        StartCoroutine(user.myMovement.LockActorMovement(timeToArrival));
        yield return new WaitForSeconds(timeToArrival);
        user.myMovement.DragActor(Vector2.zero);
        AfterMathOfCrush(user, shadowSprite);
    }
    
    /*This function handles everything that must be done after the crush has been performed.
    This will shake the camera, spawn a sin object, and destroy the shadow.*/
    void AfterMathOfCrush(Actor user, GameObject shadowSprite)
    {
        var actorColliders = user.gameObject.GetComponents<Collider2D>();
        foreach(var actorCollider in actorColliders)
        {
            actorCollider.enabled = true;
        }
        cam.Shake(2.0f, 0.2f);
        Destroy(shadowSprite);
        Instantiate(sin, user.gameObject.transform.position, Quaternion.identity);
        this.isFinished = true;
    }
}
