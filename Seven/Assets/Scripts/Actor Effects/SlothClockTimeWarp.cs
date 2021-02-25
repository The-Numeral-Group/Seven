using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlothClockTimeWarp : MonoBehaviour, ActorEffect
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("By what percentage timescale should increase when time is warped (A value of 1" + 
        " will increase timescale by 100%).")]
    public float timeFactor = 0.25f;

    //The amount of these effects that can ever exist at once
    public const float effectMax = 3;

    //The amount of these effects that currently exist
    public static float effectAmount = 0;

    //The discreet value by which timeScale will actually be changed
    private float timeAdjustment;
    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        timeAdjustment = Time.timeScale * timeFactor;
    }

    void onTriggerEnter2D(Collider2D collided){

    }

    public bool ApplyEffect(ref Actor actor){
        return false;
    }

    public void RemoveEffect(ref Actor actor){

    }
}

/*so: plan for second component: it makes a unityEvent that's invoked in on trigger enter.
Main script on clock listens to that even and keeps a bool for each hand. Once bools for both hands
are set to true, set all bools to false and apply the time effect

maybe also add visual cue to make it obvious when a hand has already been struck*/
