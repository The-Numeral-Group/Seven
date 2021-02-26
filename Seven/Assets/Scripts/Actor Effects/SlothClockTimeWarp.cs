using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlothClockTimeWarp : MonoBehaviour
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("All of the objects that are considered when activating a timewarp")]
    public List<GameObject> colliderObjects;

    [Tooltip("By what percentage timescale should increase when time is warped (A value of 1" + 
        " will increase timescale by 100%).")]
    public float timeFactor = 0.25f;

    [Tooltip("An arbitrary ActorEffectHandler to bear the slowdown effects.")]
    public ActorEffectHandler handler;

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
    reset the lit and apply the time effect*/
    void UpdateClock(GameObject obj)
    {
        trackedObjects.Remove(obj);

        //if the list is empty...
        if(trackedObjects.Count == 0)
        {
            //reset the list and apply the timewarp
            trackedObjects = new List<GameObject>(colliderObjects);
            handler.AddEffect(new SlothClockTimeWarp.TimeWarpEffect(timeAdjustment));
        }
    }

    //adds an arbitrary time change to the handler
    public void ForceTimeApplication(float timeEffect)
    {
        handler.AddEffect(new SlothClockTimeWarp.TimeWarpEffect(timeEffect));
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
    void OnTriggerEnter2D()
    {
        responseEvent.Invoke(this.gameObject);
    }
}

/*so: plan for second component: it makes a unityEvent that's invoked in on trigger enter.
Main script on clock listens to that even and keeps a bool for each hand. Once bools for both hands
are set to true, set all bools to false and apply the time effect

maybe also add visual cue to make it obvious when a hand has already been struck*/
