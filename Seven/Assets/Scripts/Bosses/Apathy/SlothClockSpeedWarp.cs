using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HandDict = System.Collections.Generic.Dictionary<UnityEngine.GameObject, int>;

public class SlothClockSpeedWarp : MonoBehaviour
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("All of the objects that are considered when activating a speedwarp")]
    public List<GameObject> colliderObjects;

    [Tooltip("An arbitrary ActorEffectHandler to bear the slowdown effects.")]
    public ActorEffectHandler handler;

    [Tooltip("The number of times each hand must be hit before a slow effect is undone.")]
    public int undoStrikes = 2;

    [Tooltip("By what percentage speed should decrease when speed is lowered (A value of 1" + 
        " will decrease speed by 100%).")]
    public float slowFactor = 0.5f;

    [Tooltip("By what percentage speed should increase when speed is raised (A value of 1" + 
        " will increase speed by 100% (that is, it will double")]
    public float fastFactor = 1f;

    [Tooltip("How long a speedup should last.")]
    public float fastDuration = 5f;

    //The discreet value by which speed was last changed
    //private float timeAdjustment;

    //The handler's ActorMovement, if it exists
    private ActorMovement handlerMove;

    //The current simpleSlow instance being used
    private SimpleSlow slow;

    //The current simpleSlow being used to speed the player up
    private SimpleSlow fast;

    //a second list which tracks which gameObjects have or have not been hit
    private HandDict trackedObjects;

    //the amount to add/subtract from the speed of the clock hands when effects are applied
    private float handRateChange = 0f;

    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        //add an observer to all of the added objects
        foreach(GameObject obj in colliderObjects)
        {
            AddObserverComponent(obj);
        }

        //create the tracker list
        trackedObjects = new HandDict();

        /*NOTE TO SELF: CHANGE TRACKEDOBJECTS TO BE A GAMEOBJECT-INT DICT SO HANDS CAN
        BE PLUGGED IN AND THE NUMBER OF TIMES THEY'VE BEEN STRUCK CAN BE PULLED OUT. OPERATES
        MUCH AS NORMAL.*/

        Component movTemp;
        if(!handler.gameObject.TryGetComponent(typeof(ActorMovement), out movTemp))
        {
            Debug.LogError("SlothClockSpeedWarp: No actor movement on handler! Speed changes" +
                " may not be applied correctly!");
        }
        handlerMove = (movTemp as ActorMovement);

        //make the slow object
        slow = new SimpleSlow(handlerMove.speed * slowFactor);

        //make the fast object
        fast = new SimpleSlow(-handlerMove.speed * (fastFactor + 1f));
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
        //if the fast has been applied...
        if(handler.EffectInstancePresent(fast))
        {
            //do nothing
            return;
        }
        //if the slow has been applied...
        else if(handler.EffectInstancePresent(slow))
        {
            ClockHandLogic(obj);
        }
        //if neither has been applied
        else
        {
            //apply the fast for a limited time, if it's not already there
            if(!handler.EffectInstancePresent(fast))
            {
                handler.AddTimedEffect(fast, fastDuration);
            }
        }
    }

    void ClockHandLogic(GameObject obj)
    {
        //reduce the number of hits on that object
        if(trackedObjects.ContainsKey(obj))
        {
            //and if that number is now 0, remove it
            if(trackedObjects[obj] == 0)
            {
                trackedObjects.Remove(obj);
            }
            ///DEBUG
            Debug.Log("ColObj " + obj.name + " activated");
            ///DEBUG

            trackedObjects[obj] -= 1;
        }

        //if the list is empty...
        if(trackedObjects.Count == 0)
        {
            ///DEBUG
            Debug.Log("SPEED CLEAN");
            ///DEBUG
            handler.SubtractEffect(slow);
            ResetTrackObjects();        
        }
    }

    void ResetTrackObjects()
    {
        //reset the list and reset the speedwarp
        foreach(GameObject hand in colliderObjects)
        {
            trackedObjects.Add(hand, undoStrikes);
        }
    }   

    public void ForceSlow()
    {
        //subtract the fast, if it exists
        handler.SubtractEffect(fast);


        //apply the slow, assuming it isn't already there
        if(!handler.EffectInstancePresent(slow))
        {
            handler.AddEffect(slow);
        }
    }
}

//This is the exact same thing as SlothClockObserver, but I can't repeat type names 'cause
//C# said that's da rules
internal class SlothSpeedClockObserver : MonoBehaviour
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

