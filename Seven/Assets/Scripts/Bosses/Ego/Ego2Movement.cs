using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ego2Movement : ActorMovement
{
    //FIELDS---------------------------------------------------------------------------------------
    [Header("Teleport Settings")]
    
    [Tooltip("How long Ego should stay out of reality between the start and end of" + 
        " a teleport.")]
    public float intangibleTime = 0.1f;

    [Tooltip("How long it should take for Ego to shift in and out of reality during a" + 
        " teleport (this will be replaced by an animation later).")]
    public float debugTeleShiftTime = 0.1f;

    [Header("Static Teleport Settings")]
    [Tooltip("How long something should stay out of reality between the start and end of" + 
        " a teleport.")]
    [SerializeField]
    private static float s_intangibleTime = 0.1f;

    [Tooltip("How long it should take for something to shift in and out of reality during a" + 
        " teleport (this will be replaced by an animation later).")]
    [SerializeField]
    private static float s_debugTeleShiftTime = 0.1f;

    [Tooltip("How far a back a teleport destination should be shifted if it ends up in a wall." + 
        " Enter 0f or less to have it be based on the user's size.")]
    [SerializeField]
    private static float s_invalidDestCorrect = 0.25f;


    //METHODS--------------------------------------------------------------------------------------
    /*The only thing being added to movement is Ego's teleport. Makes user invisible, disables
    thier collider (if any), disables their health (if any), and then sets their 
    position to be the argument, wherever that is. After arriving, the colliders and damage
    are restored.
    
    Teleports will not check if the target destination has enough space*/
    public IEnumerator EgoTeleport(Vector3 dest)
    {
        //Step 0: Get a destination that doesn't have something in it
        Vector3 destination = Ego2Movement.GetSafeLocation(
            dest, 
            this.gameObject.transform.position, 
            this.gameObject.GetComponent<Collider2D>()
        );

        //Step 1: Disable Colliders and Healths
        //because ActorMovement isn't guarunteed to have a host, we check directly for health
        var collider = this.gameObject.GetComponent<Collider2D>();
        var health = this.gameObject.GetComponent<ActorHealth>();
        if(collider){collider.enabled = false;}
        if(health){collider.enabled = false;}

        ///DEBUG
        //Step 2: Fade out the teleporter's alpha channel
        var renderer = this.gameObject.GetComponent<SpriteRenderer>();
        var fadeTime = 0.0f;

        //standard coroutine fade
        while(fadeTime < debugTeleShiftTime)
        {
            renderer.color = new Color(
                renderer.color.r,
                renderer.color.g,
                renderer.color.b,
                Mathf.Lerp(1f, 0f, fadeTime / debugTeleShiftTime)
            );
            fadeTime += Time.deltaTime;
            yield return null;
        }
        //force to invisible
        renderer.color = new Color(
                renderer.color.r,
                renderer.color.g,
                renderer.color.b,
                0f
        );
        ///DEBUG
        //Step 2: Animate the teleport
        //this.gameObject.GetComponent<ActorAnimationHandler>()?.TrySetTrigger("ego_teleport");

        //Step 2.5: spend some time out of reality
        yield return new WaitForSeconds(intangibleTime);

        //Step 3: actually teleport
        this.gameObject.transform.position = destination;

        ///DEBUG
        //Step 4: Fade in the teleporter's alpha channel
        fadeTime = 0.0f;

        //standard coroutine fade
        while(fadeTime < debugTeleShiftTime)
        {
            renderer.color = new Color(
                renderer.color.r,
                renderer.color.g,
                renderer.color.b,
                Mathf.Lerp(0f, 1f, fadeTime / debugTeleShiftTime)
            );
            fadeTime += Time.deltaTime;
            yield return null;
        }
        //force to visible
        renderer.color = new Color(
                renderer.color.r,
                renderer.color.g,
                renderer.color.b,
                1f
        );
        ///DEBUG

        //Step 5: re-enable collisions and health
        if(collider){collider.enabled = true;}
        if(health){collider.enabled = true;}
    }

    /*The exact same method, except it will run teleportation logic for any
    object. Again, Teleports will not check if the target destination has enough space*/
    public static IEnumerator EgoTeleport(Vector3 dest, GameObject user)
    {
        //Step 0: Get a destination that doesn't have something in it
        Vector3 destination = Ego2Movement.GetSafeLocation(
            dest, 
            user.transform.position, 
            user.GetComponent<Collider2D>()
        );

        //Step 1: Disable Colliders and Healths
        //because ActorMovement isn't guarunteed to have a host, we check directly for health
        var collider = user.gameObject.GetComponent<Collider2D>();
        var health = user.gameObject.GetComponent<ActorHealth>();
        if(collider){collider.enabled = false;}
        if(health){collider.enabled = false;}

        ///DEBUG
        //Step 2: Fade out the teleporter's alpha channel
        var renderer = user.gameObject.GetComponent<SpriteRenderer>();
        var fadeTime = 0.0f;

        //standard coroutine fade
        while(fadeTime < s_debugTeleShiftTime)
        {
            renderer.color = new Color(
                renderer.color.r,
                renderer.color.g,
                renderer.color.b,
                Mathf.Lerp(1f, 0f, fadeTime / s_debugTeleShiftTime)
            );
            fadeTime += Time.deltaTime;
            yield return null;
        }
        //force to invisible
        renderer.color = new Color(
                renderer.color.r,
                renderer.color.g,
                renderer.color.b,
                0f
        );
        ///DEBUG

        //Step 2.5: spend some time out of reality
        yield return new WaitForSeconds(s_intangibleTime);

        //Step 3: actually teleport
        user.gameObject.transform.position = destination;

        ///DEBUG
        //Step 4: Fade in the teleporter's alpha channel
        fadeTime = 0.0f;

        //standard coroutine fade
        while(fadeTime < s_debugTeleShiftTime)
        {
            renderer.color = new Color(
                renderer.color.r,
                renderer.color.g,
                renderer.color.b,
                Mathf.Lerp(0f, 1f, fadeTime / s_debugTeleShiftTime)
            );
            fadeTime += Time.deltaTime;
            yield return null;
        }
        //force to visible
        renderer.color = new Color(
                renderer.color.r,
                renderer.color.g,
                renderer.color.b,
                1f
        );
        ///DEBUG

        //Step 5: re-enable collisions and health
        if(collider){collider.enabled = true;}
        if(health){collider.enabled = true;}
    }

    static Vector3 GetSafeLocation(Vector3 potentialDest, Vector3 startPoint, Collider2D collider)
    {
        //if there is no collider, then all locations are safe!
        if(!collider)
        {
            return potentialDest;
        }

        //save the destination for modification
        Vector3 realDest = potentialDest;
        //save the direction from start to dest
        Vector3 teleDirection = (potentialDest - startPoint).normalized;
        //save the largest "radius" of the teleporting bounds. We can ignore z size (depth)
        
        float colSize;

        if(s_invalidDestCorrect > 0f)
        {
            colSize = s_invalidDestCorrect;
        }
        else
        {
            var colBounds = collider.bounds.extents;
            colSize = colBounds.x > colBounds.y ? colBounds.x : colBounds.y;
        }

        Debug.Log($"Trying location {potentialDest} from {startPoint}");

        /*Cast a sphere into the play space centered on realDest with a radius of colSize. If
        there is anything in the sphere at all, move potentialDest backwards towards the start
        point by a distance of colSize. If this would put the destination behind the start, just
        teleport in place*/
        Collider2D lastObj = Physics2D.OverlapCircle(realDest, colSize);
        while(lastObj)
        {
            Debug.Log($"Blocked by {lastObj.gameObject.name}. Shifting to {realDest -= (teleDirection * colSize)}");
            
            /*If we've gotten here, that means there isn't enough space at the destination.
            So, move back a bit*/
            realDest -= (teleDirection * colSize);
            //Debug.DrawLine(potentialDest, realDest, Color.red, 100f);
            Debug.DrawRay(realDest, teleDirection * (-colSize));
            Debug.DrawLine(Vector3.zero, realDest, Color.green, 100f);

            /*if the direction from the start to the new dest ISN'T the direction from the start
            to the original dest, just return the start point*/
            if(teleDirection != (realDest - startPoint).normalized)
            {
                Debug.Log($"Defaulting to {startPoint}");
                return startPoint;
            }

            lastObj = Physics2D.OverlapCircle(realDest, colSize);
        }

        //if we're here, that means realDest is valid.
        return realDest;
    }
}
