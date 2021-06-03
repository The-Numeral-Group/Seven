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

    [Tooltip("How fast Ego travels when Swaggering.")]
    public float swaggerSpeed = 4f;

    [Tooltip("Hpw fast Ego travels when Sprinting.")]
    public float sprintSpeed = 6f;

    [Tooltip("How long Ego will swagger for before just sprinting towards the player.")]
    public float swaggerDuration = 3f;

    [Header("Attacks")]
    [Tooltip("Controls how many times should Ego use normal attacks before using it's special.")]
    public int specialAttackGate = 7;

    [Tooltip("How far away the player should be before Ego charges at them.")]
    public float chargeRange = 60f;

    [Tooltip("How far away the player should be before Ego launches" + 
        " a short-range attack at them.")]
    public float punchRange = 25f;

    [Tooltip("The ability Object Ego should drop when it dies after the player has sinned.")]
    public GameObject abilityDropObject;

    /*reference to this script. Abilities need a reference to their user to be used, but 'this'
    is read-only, and thus can't used. We need to manually reobtain a writable reference.*/
    private Actor ego;

    //reference to player for movement tracking and convinience
    private Actor player;

    //Ego1's ActorAnimationHandler casted to its derived type for domain-specific functions
    private Ego1AnimationHandler egoAnims;

    //whatever ability Ego is currently using, if any
    private ActorAbility currAbility;

    //internal counter to track when Ego should use its special
    private int specialAttackCounter = 0;

    //the timer that runs whether or not Ego should Swagger or Sprint
    private IEnumerator sprintTimer;

    //the gamesave manager for accurately judging ego's aliveness
    private GameSaveManager gameSaveManager;

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

        //save ego's sprint-related timer
        sprintTimer = ResetSprint();

        //save ego's AAH, if it's the ego1 specific version
        if(this.myAnimationHandler is Ego1AnimationHandler)
        {
            egoAnims = (this.myAnimationHandler as Ego1AnimationHandler);
        }

        //save the gamesave manager
        gameSaveManager = GameObject.Find("GameSaveManager").GetComponent<GameSaveManager>();
        if(gameSaveManager == null)
        {
            Debug.LogWarning("Ego1Actor: Ego can't find the gameSave!");
        }
        //if ego has already been defeated...
        else if(gameSaveManager.getBoolValue(15) == true)
        {
            //Auto-kill ego
            StartCoroutine(Die());
            return;
        }

        //set EgoSin's SinMax to be the sin gate
        EgoSin.sinMax = sinGate;
        EgoSin.applicationCount = 0;

        StartCoroutine(sprintTimer);
        StartCoroutine(BossBehaviour());
    }

    // fixed update is called every phyiscs sim tick
    void FixedUpdate()
    {
        //move Ego's face anchor towards the player
        /*DoActorUpdateFacing(
            (player.gameObject.transform.position - this.gameObject.transform.position).normalized
        );*/

        //update Ego's animations
        this.myAnimationHandler.animateWalk();
    }

    /*Ego's computational driver. Initially I (Thomas) intended to recycle PrideActor, but I like
    the coroutine-based behaviours so much that I'm doing it again.
    
    Ego's behaviour is to check its distance to the player and then, based on those distances,
    attack or walk towards the player.*/
    IEnumerator BossBehaviour()
    {
        while(true)
        {
            //Step 0: Animate ego's idle/walk, if needed
            this.myAnimationHandler.animateWalk();

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
            
            //Step 2.4: If the player is close, save the close
            else if(dist <= punchRange)
            {
                currAbility = 
                    this.myAbilityInitiator.abilities[AbilityRegister.PRIDE_CLOSE_ATTACK];
                Debug.Log("Ego1Actor: Close");
            }
            //Step 2.3: If the player is far away, save the far
            else if(dist > chargeRange)
            {
                currAbility = 
                    this.myAbilityInitiator.abilities[AbilityRegister.PRIDE_FAR_ATTACK];
                Debug.Log("Ego1Actor: Far");
            }

            //Step 3: Attack
            //If there is an attack..
            if(currAbility && currAbility.getUsable())
            {
                //Reset Ego's sprinting/swaggering
                StopCoroutine(sprintTimer);
                this.myMovement.speed = swaggerSpeed;
                egoAnims?.SetSwagger(true);
               
                //stop moving
                this.myMovement.MoveActor(Vector2.zero);
                
                //invoke it, count it, and wait for it
                ++specialAttackCounter;
                currAbility.Invoke(ref ego, player);
                yield return new WaitUntil( () => currAbility.getIsFinished());

                //starting the swagger once again
                StartCoroutine(ResetSprint());
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

    IEnumerator ResetSprint()
    {
        this.myMovement.speed = swaggerSpeed;
        egoAnims?.SetSwagger(true);
        yield return new WaitForSeconds(swaggerDuration);
        this.myMovement.speed = sprintSpeed;
        egoAnims?.SetSwagger(false);
    }

    //Ego1 will switch to Ego2 upon death.
    public override void DoActorDeath()
    {
        //if the player hasn'y sinned enough...
        if(EgoSin.applicationCount < sinGate)
        {
            //save the lack of sin
            gameSaveManager.setBoolValue(false, 14);

            ///DEBUG
            Debug.Log("Ego1Actor: Phase change!");
            ///DEBUG
            this.gameObject.SendMessage(
                "NextPhase", 
                new System.Tuple<Actor, System.Action<Actor>>(
                    this, 
                    (actor) => 
                        {actor.gameObject.transform.position = this.gameObject.transform.position;}
                )
            );
        }
        //if they haven't...
        else
        {
            //save the sin
            gameSaveManager.setBoolValue(true, 14);

            ///just destroy this Ego
            StartCoroutine(Die());
        }
    }

    //yields death to next frame so UI can catch up
    //this can be deletaed when a real death effect is added
    IEnumerator Die()
    {
        //save Ego's death
        gameSaveManager.setBoolValue(true, 15);

        //create an ability object and set it's flag to 8 to reference Ego's ability
        //assuming the player hasn't grabbed it already
        if(gameSaveManager.getBoolValue(8) == false)
        {
            var abilityObj = Instantiate(
                abilityDropObject, 
                this.gameObject.transform.position, 
                Quaternion.identity
            ).GetComponent<AbilityPickup>();
            abilityObj.gameSaveAbilityPickupIndex = 8;
            abilityObj.gameSaveManager = gameSaveManager;
        }

        yield return null;

        //fukkin die
        Destroy(this.gameObject);
    }

    void OnDestroy()
    {
        //reset the sin counter
        EgoSin.applicationCount = 0;
    }
}
