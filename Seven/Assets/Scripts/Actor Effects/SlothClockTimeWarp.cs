using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlothClockTimeWarp : MonoBehaviour
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("All of the objects that are considered when activating a timewarp")]
    public List<GameObject> colliderObjects;

    [Tooltip("An arbitrary ActorEffectHandler to bear the slowdown effects.")]
    public ActorEffectHandler handler;

    [Tooltip("By what percentage timescale should increase when time is warped (A value of 1" + 
        " will increase timescale by 100%).")]
    public float timeFactor = 0.25f;

    [Tooltip("How long a time warp applied by the clock will last.")]
    public float timeDuration = 10f;

    //The lowest the timescale can be driven by this effect
    public const float effectmin = 0.1f;

    //The amount of these effects that currently exist
    //public static float effectAmount = 0;

    //The discreet value by which timeScale will actually be changed
    private float timeAdjustment;

    //a second list which tracks which gameObjects have or have not been hit
    private List<GameObject> trackedObjects;
    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        //calculate timeAdjustment
        timeAdjustment = Time.timeScale * timeFactor;

        //add an observer to all of the added objects
        foreach(GameObject obj in colliderObjects)
        {
            AddObserverComponent(obj);
        }

        //create the tracker list
        trackedObjects = new List<GameObject>(colliderObjects);
    }

    /*adds an observer component to the gameobject, allowing this script to read
    events off of it without the gameobject needing a reference at compiletime.*/
    void AddObserverComponent(GameObject obj)
    {
        //Step 1: create the unity event to watch for
        var ue = new ObjectSelfEditEvent();

        //Step 2: set up UpdateClock to trigger when the event is invoked
        ue.AddListener(UpdateClock);

        //Step 3: create a new observer component
        //Step 4: and give the component the new event
        obj.AddComponent<SlothClockObserver>().responseEvent = ue;

        //Step 5: assume the object has a collider component that can listen to attacks
    }

    /*updates the clock's tracker list. If this results in the list being empty, then
    reset the list and apply the time effect*/
    void UpdateClock(GameObject obj)
    {
        trackedObjects.Remove(obj);
        ///DEBUG
        Debug.Log("ColObj " + obj.name + " activated");
        ///DEBUG

        //if the list is empty...
        if(trackedObjects.Count == 0)
        {
            //reset the list and apply the timewarp
            trackedObjects = new List<GameObject>(colliderObjects);
            ///DEBUG
            Debug.Log("TIME WARP");
            ///DEBUG
            //handler.AddEffect(new SlothClockTimeWarp.TimeWarpEffect(timeAdjustment));
            ForceTimedTimeApplication(timeAdjustment, timeDuration);
        }
    }

    //adds an arbitrary time change to the handler
    public void ForceTimeApplication(float timeEffect)
    {
        handler.AddEffect(new SlothClockTimeWarp.TimeWarpEffect(timeEffect));
    }

    //adds an arbitrary time change to the handler with a set duration
    public void ForceTimedTimeApplication(float timeEffect, float duration)
    {
        handler.AddTimedEffect(new SlothClockTimeWarp.TimeWarpEffect(timeEffect), duration);
    }

    //Reset Time.timeScale to 1... just in case...
    //might have other fallback/error handling responsibilities later
    public void EmergencyTimeReset()
    {
        Debug.LogWarning("SlothClockTimeWarp: timescale forcibly reset to 1");
        Time.timeScale = 1f;
    }

    /*WHAT THE SHIT IT'S A WHOLE DIFFERENT CLASS DECLARATION
    Yes! To allow for individual instances of time changes to have different values,
    a private class is created to be able to handle each effect*/
    private class TimeWarpEffect : ActorEffect 
    {
        //FIELDS-----------------------------------------------------------------------------------
        //The actual value that got added to the timescale by this class instance
        private float timeChange = 0;

        //METHODS----------------------------------------------------------------------------------
        //Sets the amount we actually want to change time by
        public TimeWarpEffect(float timeChange)
        {
            this.timeChange = timeChange;
        }

        //LETS DO THE TIMEWARP AGAIN
        public bool ApplyEffect(ref Actor actor)
        {
            /*first, check if doing this application would underball the minimum
            timescale. If it would, abort the change*/
            if(Time.timeScale - timeChange < SlothClockTimeWarp.effectmin)
            {
                Debug.LogWarning($"TimeWarpEffect in SlothClockTimeWarp: attempted timescale" + 
                    " change from {Time.timeScale} to {Time.timeScale} goes under the" + 
                        " minimum of {SlothClockTimeWarp.effectmin}.");

                return false;
            }
            else
            {
                Time.timeScale += timeChange;
            }

            return true;
        }

        //undoes the timescale effect
        public void RemoveEffect(ref Actor actor)
        {
            Time.timeScale -= timeChange;
        }
    }
}

internal class SlothClockObserver : MonoBehaviour
{
    //FIELDS---------------------------------------------------------------------------------------
    //The event that gets fired by this observer
    [System.NonSerialized]
    public ObjectSelfEditEvent responseEvent = null;

    //might want to set this up a little differently, as OTE2D may also
    //respond to entry from other objects, like things walking into it?
    //FIELDS---------------------------------------------------------------------------------------
    void Start()
    {
        //if the collider doesn't have an ActorHealth, add one that has 100% damage resistance
        if(this.gameObject.GetComponent<ActorHealth>() == null)
        {
            var newHealth = this.gameObject.AddComponent<ActorHealth>();
            newHealth.damageResistance = 1f;
        }
    }

    void DoActorDamageEffect()
    {
        responseEvent.Invoke(this.gameObject);
    }
}

/*so: plan for second component: it makes a unityEvent that's invoked in on trigger enter.
Main script on clock listens to that even and keeps a bool for each hand. Once bools for both hands
are set to true, set all bools to false and apply the time effect

maybe also add visual cue to make it obvious when a hand has already been struck*/
