using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrideActor : Actor
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("Put every GameObject that the player needs to destroy to kill Pride in this list.")]
    public List<ActorWeakPoint> weakSpots;

    [Tooltip("Controls how many times should Pride use normal attacks before using it's special.")]
    public int specialAttackGate = 7;

    [Tooltip("How far away the player should be before Pride launches a shockwave at them.")]
    public float waveRange = 60f;

    [Tooltip("How close the player should be before Pride tries to punch them.")]
    public float punchRange = 25f;

    [Tooltip("How upscaled (how many times bigger) Pride is at max health (will decrease" +  
        " proportionally based on number of weak spots).")]
    public float maxSize = 10f;

    [Tooltip("How upscaled (how many times bigger) Pride is at minimum health (will decrease" +  
        " proportionally based on number of weak spots).")]
    public float minSize = 5f;

    [Tooltip("How much slower Pride is than the player.")]
    public float speedModifier = -5f;

    [Tooltip("Whether or not Pride should override his default speed with the player's speed" + 
        " but slower.")]
    public bool overrideDefaultSpeed = true;

    //reference to player for movement tracking and convinience
    private Actor player;

    //internal counter to track when Pride should use its special
    private int specialAttackCounter = 0;

    //METHODS--------------------------------------------------------------------------------------
    /*Aquires references to needed actor components, then sets Pride's speed based on
    overrideDefaultSpeed and speedModifer*/
    new void Start()
    {
        //get component references
        base.Start();

        //find player...
        var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];

        //and save it's actor, if possible
        if(playerObject == null)
        {
            Debug.LogWarning("PrideActor: Pride can't find the player!");
        }
        else
        {
            player = playerObject.GetComponent<Actor>();
        }

        //adjust speed if we (design) want the "almost as fast as player" thing
        if(overrideDefaultSpeed)
        {
            this.myMovement.speed = player.myMovement.speed + speedModifier;
        }

        //ensure that the ActorWeakPoints used to hurt Pride are correctly assigned
        foreach(ActorWeakPoint weakSpot in weakSpots)
        {
            weakSpot.ownerHealth = this.myHealth;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
