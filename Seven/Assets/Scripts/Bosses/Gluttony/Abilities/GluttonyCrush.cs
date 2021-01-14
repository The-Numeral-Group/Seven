using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GluttonyCrush : ActorAbilityFunction<Actor, int>
{
    //Shadow object should for now have a cript that will tie it's movement with Gluttony.
    public GameObject startingShadowSprite;
    //the offet being used to spawn the shadowSprite
    public Vector2 distanceFromActor = new Vector2(0, -6);
    //The sin object which will be spawned after the crush.
    public GameObject sin;
    //The actor this move is meant to target
    public Actor targetActor;
    public float fallSpeed = 10;
    public float jumpSpeed = 10;
    private ShakeCamera cam = null;
    private float jumpDuration = 1f;
    private float trackDuration = 2f;
    private float crushDelay = 1f;
    private float totalAbilityDuration;

    //How high gluttony jumps into the air;
    private float verticalOffset = 120f;

    // Start is called before the first frame update
    void Start()
    {
        //Compount all the coroutine delays to get the total duration of the ability.
        this.totalAbilityDuration = this.jumpDuration + this.trackDuration + this.crushDelay;
        var camObjects = FindObjectsOfType<ShakeCamera>();
        if (camObjects.Length > 0)
        {
            cam = camObjects[0];
        }
        else
        {
            Debug.Log("Gluttony Crush: does not have access to a camera that can shake");
        }

        if (this.targetActor == null)
        {
            Debug.Log("Gluttony Crush: No valid ActorMovement target/object provided");
        }
    }

    //Similar to AbilityFunctions base cooldown, but usable is set to true outside of this function.
    //The move should only be usable when the move is completed.
    public override IEnumerator coolDown(float cooldownDuration)
    {
        usable = false;
        yield return new WaitForSeconds(cooldownDuration);
        //usable = true;
    }

    //Used as a way to start the three-part coroutine that makes up the ability.
    //Initiates the first portion which is JumpUp.
    //Passes the direction to jump towards.
    protected override int InternInvoke(Actor[] args)
    {
        Vector2 destination = new Vector2(this.targetActor.transform.position.x, this.targetActor.transform.position.y + this.verticalOffset);
        Vector2 direction = (destination - new Vector2(args[0].transform.position.x, args[0].transform.position.y)).normalized;
        direction = direction * this.jumpSpeed;
        StartCoroutine(args[0].myMovement.LockActorMovement(this.totalAbilityDuration));
        StartCoroutine(JumpUp(args[0], direction));
        return 0;
    }

    //Part 1 of the 3 steps to perform crush.
    //Moves the ability user to in the defined direction.
    //Instatiates an object to represent its final destination.
    //Calls both Part 2 and 3 of this process.
    private IEnumerator JumpUp(Actor user, Vector2 direction)
    {
        user.myMovement.DragActor(direction);
        yield return new WaitForSeconds(this.jumpDuration);
        GameObject shadowSprite = Instantiate(this.startingShadowSprite, this.targetActor.transform.position, Quaternion.identity);
        IEnumerator trackTarget = TrackTargetWithShadow(user);
        IEnumerator crush = Crush(user, shadowSprite, trackTarget);
        StartCoroutine(trackTarget);
        StartCoroutine(crush);
    }

    //Part 2 of the 3 steps to perform crush.
    //Tracks the targets movement by having the ability user mimic it's user movementdirection and speed.
    //Loops infinitely, but is stopped by Part 3.
    private IEnumerator TrackTargetWithShadow(Actor user)
    {
        //WORK IS PAUSED UNTIL I SORT OUT SOME METHODS REGARDING DRAGACTOR
        while(true)
        {
            yield return new WaitForFixedUpdate();
            user.myMovement.DragActor(targetActor.myMovement.movementDirection * targetActor.myMovement.speed);
        }
    }

    //Part 3 of the 3 steps to perform crush.
    //Stops part 2 after a specified duration as passed.
    //Will then move the ability user towards the shadowSprites position.
    //Starts the aftermath function which performs the abilities efx.
    //RIGHT NOW CRUSH USES TEMPORARY CODE TO MOVE THE OBJECT UNTIL A BETTER WAY IS IMPLEMENTED WHICH UTILIZES DRAGACTOR
    private IEnumerator Crush(Actor user, GameObject shadowSprite, IEnumerator toStop)
    {
        yield return new WaitForSeconds(this.trackDuration);
        StopCoroutine(toStop);
        user.myMovement.DragActor(Vector2.zero);
        yield return new WaitForSeconds(this.crushDelay);
        
        //The folliwing movement handling is placeholder code.
        //movement should ideally be handled by DragACtor.
        //Will change once I (Ram) think of a better way to drag an actor to a specified location.
        Vector2 desiredPos = shadowSprite.transform.position;
        Vector2 currentPos = this.gameObject.transform.position;
        for (float i = 0.0f; i < 1.0f; i += Time.deltaTime * this.fallSpeed)
        {
            Vector2 result = Vector2.Lerp(currentPos, desiredPos, i);
            this.gameObject.transform.position = new Vector3(result.x, result.y, this.gameObject.transform.position.y);
            yield return null;
        }
        //Above is temporary code to facilitate movement.

        AfterMathOfCrush(shadowSprite);
    }
    
    //This function handles everything that must be done after the crush has been performed.
    //This will shake the camera, spawn a sin object, and destroy the shadow.
    private void AfterMathOfCrush(GameObject shadowSprite)
    {
        this.cam.CameraShake(2.0f, 0.2f);
        Destroy(shadowSprite);
        Instantiate(this.sin, this.gameObject.transform.position, Quaternion.identity);
        usable = true;
    }
}
