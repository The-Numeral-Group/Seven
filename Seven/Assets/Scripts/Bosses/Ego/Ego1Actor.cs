using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This was copied almost wholesale from PrideActor. Some comments may still reference Pride or
its attacks, such as how the far charge is sometimes called a wave projectile, because that's
what went in that slot of Pride originally.*/

public class Ego1Actor : Actor
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("The amount of EgoSin objects the player must accumulate (how many times they need" + 
        " to appeal to the crowd) for Ego to enter Phase 2 upon death.")]
    public int sinGate = 3;

    [Header("Attacks")]
    [Tooltip("Controls how many times should Ego use normal attacks before using it's special.")]
    public int specialAttackGate = 7;

    [Tooltip("How far away the player should be before Ego charges at them.")]
    public float chargeRange = 60f;

    [Tooltip("How far away the player should be before Ego launches" + 
        " a short-range attack at them.")]
    public float punchRange = 25f;

    /*reference to this script. Abilities need a reference to their user to be used, but 'this'
    is read-only, and thus can't used. We need to manually reobtain a writable reference.*/
    private Actor ego;

    //reference to player for movement tracking and convinience
    private Actor player;

    //whatever ability Ego is currently using, if any
    private ActorAbility currAbility;

    //internal counter to track when Ego should use its special
    private int specialAttackCounter = 0;

    //METHODS--------------------------------------------------------------------------------------
    /*Aquires references to needed actor components, then sets Ego's speed based on
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
            Debug.LogWarning("Ego1Actor: Ego can't find the player!");
        }
        else
        {
            player = playerObject.GetComponent<Actor>();
        }

        //save ego's own actor
        ego = this.gameObject.GetComponent<Ego1Actor>();

        StartCoroutine(BossBehaviour());
    }

    /*Ego's computational driver. Initially I (Thomas) intended to recycle PrideActor, but I like
    the coroutine-based behaviours so much that I'm doing it again.
    
    Ego's behaviour is to check its distance to the player and then, based on those distances,
    attack or walk towards the player.*/
    IEnumerator BossBehaviour()
    {
        while(true)
        {
            //Step 1: Check how far from the player Ego is
            var dist = Mathf.Abs(Vector2.Distance(
                player.transform.position, 
                this.gameObject.transform.position
            ));

            //Step 2: Decide which attack to do
            //Step 2.1: null out currAbility
            currAbility = null;

            //Step 2.2: If this is attack # attackGate, save the special
            if(specialAttackCounter >= specialAttackGate)
            {
                currAbility = 
                    this.myAbilityInitiator.abilities[AbilityRegister.PRIDE_SPECIAL];
                specialAttackCounter = 0;
                Debug.Log("Ego1Actor: Special");
            }
            //Step 2.3: If the player is far away, save the far
            else if(dist >= chargeRange)
            {
                currAbility = 
                    this.myAbilityInitiator.abilities[AbilityRegister.PRIDE_FAR_ATTACK];
                Debug.Log("Ego1Actor: Far");
            }
            //Step 2.4: If the player is close, save the close
            else if(dist <= punchRange)
            {
                currAbility = 
                    this.myAbilityInitiator.abilities[AbilityRegister.PRIDE_CLOSE_ATTACK];
                Debug.Log("Ego1Actor: Close");
            }

            //Step 3: Attack
            //If there is an attack..
            if(currAbility && currAbility.getUsable())
            {
                //stop moving
                this.myMovement.MoveActor(Vector2.zero);
                
                //invoke it, count it, and wait for it
                ++specialAttackCounter;
                currAbility.Invoke(ref ego, player);
                yield return new WaitUntil( () => currAbility.getIsFinished());
            }
            //If there isn't an attack...
            else
            {
                //just step towards the player
                stepTowardsPlayer();
                yield return null;
            }
        }

    }

    /*Calculates where the player is, then makes one movement instance towards them.
    
    Yes, this was copied from Gluttony.*/
    void stepTowardsPlayer()
    {
        var myPos = this.gameObject.transform.position;
        var playerPos = player.gameObject.transform.position;

        /*Fun Fact! Because positions are vectors, the normalized difference between
        posB (myPos) and posA (playerPos) is the direction from posA to posB.*/
        var directionToPlayer = (playerPos - myPos).normalized;

        ///DEBUG
        //Debug.Log("Ego moving in this direction: " + directionToPlayer);
        ///DEBUG

        this.myMovement.MoveActor(directionToPlayer);
    }

    //Ego1 will switch to Ego2 upon death.
    public override void DoActorDeath()
    {
        //if the player has sinned enough...
        if(EgoSin.applicationCount >= sinGate)
        {
            ///DEBUG
            Debug.Log("Ego1Actor: Phase change!");
            ///DEBUG
            this.gameObject.SendMessage(
                "NextPhase", 
                new System.Tuple<Actor, System.Action<Actor>>(
                    this, 
                    null
                )
            );
        }
        //if they haven't...
        else
        {
            ///just destroy this Ego
            Destroy(this.gameObject);
        }
    }
}
