using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SlothRangeMarker : MonoBehaviour
{
    //FIELDS---------------------------------------------------------------------------------------
    //the various states that a marker can be in
    public enum MarkerState
    {
        STANDBY,
        ACTIVE,
        GRABACTIVE,
        POSTACTIVE
    }

    [Tooltip("The current state of this marker (state determines functionality" + 
        " of marker, changing during runtime may cause unintended effects).")]
    public MarkerState state = MarkerState.STANDBY;

    [Tooltip("The amount of damage that will be dealt if the player is standing on the marker" + 
        " during the ACTIVE state.")]
    public float strikeDamage = 2f;

    [Tooltip("How long the ACTIVE state should last in SECONDS.")]
    public float strikeDuration = (1/6f);

    [Tooltip("How long a player should stay immobile after being grabbed during the" + 
        " POSTACTIVE state in SECONDS.")]
    public float grabAttackDuration = 2f;

    [Tooltip("How long the marker should stay on the ground in the POSTACTIVE state" + 
        " (enter -1 to make the marker last until Sloth dies.)")]
    public float grabDuration = 15f;

    //the coroutine being used to handle the marker's timing
    public Coroutine timer;


    //METHODS--------------------------------------------------------------------------------------
    // what happens when something walks into this object
    void OnTriggerEnter2D(Collider2D col)
    {
        //get potential components from the actor
        var health = col.gameObject.GetComponent<ActorHealth>();
        var move = col.gameObject.GetComponent<ActorMovement>();

        //if the marker is active
        if(state == MarkerState.ACTIVE && health != null)
        {
            //hurt whoever walked in
            health.takeDamage(strikeDamage);

            //finish the marker
            ConcludeMarker();
        }
        //if the marker is grab active
        else if(state == MarkerState.GRABACTIVE && move != null)
        {
            //stop any old timings so the grab takes priority
            if(timer != null)
            {   
                StopCoroutine(timer);
            }
            //grab whoever moved into the marker
            timer = StartCoroutine(Grab(move));

            //switch to postactive so no-one else can be grabbed
            state = MarkerState.POSTACTIVE;
        }
    }

    /*Message target for making the marker switch states when it gets struck by Sloth's
    projectiles*/
    void OnActivateMarker()
    {
        //stop any old timings in case this marker is being reset
        if(timer != null)
        {
            StopCoroutine(timer);
        }

        //start new timings
        timer = StartCoroutine(MarkerTiming());
    }

    /*The state timings for the marker. Simply switches between states after waiting for
    long enough, then concludes the marker when everything is done.*/
    IEnumerator MarkerTiming()
    {
        state = MarkerState.ACTIVE;
        yield return new WaitForSeconds(strikeDuration);
        state = MarkerState.GRABACTIVE;

        if(grabDuration >= 0)
        {
            yield return new WaitForSeconds(grabDuration);
            ConcludeMarker();
        }
        else
        {
            Debug.Log("SlothRangeMarker: infinite duration marker requested. Currently doens't end" + 
                " when Sloth dies, that's being worked on.");
        }
    }

    /*The coroutine that actually handles how the player (or other target) gets grabbed.*/
    IEnumerator Grab(ActorMovement mov)
    {
        //calculate how the victim needs to be dragged
        Vector2 dragDir = new Vector2(0, 0);
        //lock actor movement and move them onto the marker
        mov.LockActorMovement(grabAttackDuration);
        mov.DragActor(dragDir);

        //automatically conclude the marker after a few seconds
        yield return new WaitForSeconds(grabAttackDuration);

        ConcludeMarker();
    }

    //what happens when the marker decides it's not needed anymore
    void ConcludeMarker()
    {
        //currently just destroys the marker
        Destroy(this.gameObject);
    }
}
