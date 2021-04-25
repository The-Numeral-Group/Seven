using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SlothActor : Actor
{
    /*Sloth is super reactive, and will thus rely on listening to the player's actions,
    rather than doing things itself.
    
    Also the start of the fight is just Sloth chin-waggin' at ya so the fight starts with
    dialogue. When it closes, the fight starts. If the player leaves the room before the
    dialogue ends, sin is applied.*/

    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("How often Sloth uses its special (if this number is 7, Sloth will special on its" + 
        " seventh attack, for example)")]
    public int attackLimit = 7;

    [Tooltip("How many seconds of must pass before" + 
        " Sloth attacks them for not standing still.")]
    public float attackDelay = 6.0f;

    [Tooltip("How close the player needs to be for Sloth/Apathy to swat them away.")]
    public float swatDistance = 5f;

    //[Tooltip("The Menu Manager that runs Sloth's dialogue")]
    //public MenuManager menuManager;

    //How many attacks Sloth has done
    private int attackCount = 0;

    //whether or not sloth is actively fighting the player
    private bool activated = false;

    //player reference for easy access
    private Actor player;

    //a secondary reference to this script, because 'this' is readonly and can't be passed as a ref
    private Actor sloth;

    //the ability sloth used last
    private ActorAbility currAbility;

    //an observer to watch and read the player's actions
    private SlothPlayerObserver observer;

    //the AAM used to engage the SlothClock when the fight starts
    private SlothClockMod AAM;

    //invoke this event to make sloth hostile
    //public UnityEvent activationEvent = new UnityEvent();

    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    new void Start()
    {
        //setting components
        base.Start();

        //get components for ability references
        player = GameObject.FindWithTag("Player")?.GetComponent<Actor>();
        sloth = this.gameObject.GetComponent<Actor>();

        //turn off sloth's ability to get hurt while it's talking
        //negative values are the "forever" value
        this.myHealth.SetVulnerable(false, -10f);

        //add sloth sin here
        if(player.myEffectHandler == null)
        {
            Debug.LogWarning("SlothActor: Sloth was unable to locate the player's effect" + 
                " handler during start");
            player.gameObject.GetComponent<ActorEffectHandler>().AddEffect(new SlothSin());
        }
        else
        {
            player.myEffectHandler.AddEffect(new SlothSin());
        }
        
        //create AAM object here (to reduce load/lag when starting the fight)
        AAM = new SlothClockMod();

        //initialize sloth/apathy's current ability to its close attack
        currAbility = this.myAbilityInitiator.abilities[AbilityRegister.SLOTH_PHYSICAL];
    }

    // FixedUpdate is called once per simulation tick
    void FixedUpdate()
    {
        //apparently this will make sloth face the player
        //but only when the fight is going
        if(activated)
        {
            var playerDist = Mathf.Abs(
                Vector3.Distance(
                    player.gameObject.transform.position, 
                    this.gameObject.transform.position
                )
            );

            //if the player's too close, swat them away
            if(currAbility.getIsFinished() && playerDist <= swatDistance)
            {
                ActivateAbility(this.myAbilityInitiator.abilities[AbilityRegister.SLOTH_PHYSICAL]);
                return;
            }

            this.gameObject.transform.right = 
                player.gameObject.transform.position - this.gameObject.transform.position;
        }
        
    }

    //ITS TIME
    public void ActivateSloth()
    {
        //we are officially throwing hands
        activated = true;

        //remove sloth sin here
        player.myEffectHandler.SubtractEffectByType<SlothSin>();

        //turn on sloth's ability to get hurt while it's talking
        //negative values are the "forever" value
        this.myHealth.SetVulnerable(true, -10f);

        //create an observer and places it on the player
        observer = player.gameObject.AddComponent<SlothPlayerObserver>();

        //setting the observer's attack delay
        observer.attackDelay = attackDelay;

        //adds listeners that tell Sloth to attack with a certain ability
        observer.playerStandStill = new UnityEvent();
        observer.playerStandStill.AddListener(delegate()
        {
            ///DEBUG
            Debug.Log("SlothActor: Sloth wants to attack because you wouldn't stand still");
            ///DEBUG
            //ActivateAbility(this.myAbilityInitiator.abilities[AbilityRegister.SLOTH_PHYSICAL]);
        });

        observer.playerMove = new UnityEvent();
        observer.playerMove.AddListener(delegate()
        {
            ///DEBUG
            //Debug.Log("SlothActor: Sloth wants to attack because you started moving");
            ///DEBUG
            ActivateAbility(this.myAbilityInitiator.abilities[AbilityRegister.SLOTH_RANGE]);
        });

        //engage the Sloth Clock being used by Sloth's Special Ability
        AAM.ModifyAbility(this.myAbilityInitiator.abilities[AbilityRegister.SLOTH_SPECIAL]);
    }

    /*Makes Sloth the attack the player with a specific ability. Sloth attacks this way to maintain
    functionality based off of serial ability usage, even though its abilities are triggered by
    event based stimuli, rather than direct data comparison*/
    void ActivateAbility (ActorAbility ability)
    {
        //if the ability is off cooldown...
        if(ability.getUsable())
        {
            //increase the attack count
            ++attackCount;

            //if it's less than the limit, just us the ability
            if(attackCount < attackLimit)
            {
                currAbility = ability;
                ability?.Invoke(ref sloth, player);
            }
            //if it isn't less than the limit, then it's time to TIMEWARP
            else
            {
                attackCount = 0;
                currAbility = this.myAbilityInitiator.abilities[AbilityRegister.SLOTH_SPECIAL];
                this.myAbilityInitiator.abilities[AbilityRegister.SLOTH_SPECIAL].Invoke(ref sloth);
            }
        }
    }

    //What happens when this actor dies
    public override void DoActorDeath()
    {
        //clean the observer off of the player
        Destroy(observer);

        //destroy sloth (this is temp, will be replaced with an animation)
        Destroy(this.gameObject);
    }

    /*private class for accessing the clock sloth uses to turn its time slowing effect on when
    the fight begins.*/
    private class SlothClockMod : ActorAbilityModifier 
    {
        //initializes changes dict as required
        public SlothClockMod () : base ()
        {
            InitializeChanges(this.changes);
        }

        /*adds the needed change to the ability: setting the "enabled" field of the
        "clock" field of the ability to true.*/
        protected override void InitializeChanges(Dictionary<string, Action<dynamic>> changes)
        {
            Action<dynamic> del = new Action<dynamic> ( (dynamic arg) => {
                if(ActorAbilityModifier.DoesMemberExist(arg.clock, "enabled"))
                {
                    arg.clock.enabled = true;
                    arg.clock.gameObject.SetActive(true);
                }
            });

            changes.Add("clock", del);
        }
    }
}

//this class reads the player's actions and calls methods on SlothActor to drive Sloth's attacks
internal class SlothPlayerObserver : MonoBehaviour
{
    //FIELDS---------------------------------------------------------------------------------------
    //The Sloth instance that this observer gives method calls to
    //[System.NonSerialized]
    //public SlothActor reciever = null;
    [Tooltip("How many seconds of must pass before" + 
        " Sloth attacks them for not standing still.")]
    public float attackDelay = 6.0f;

    /*it should be noted that these methods are not ActorAbility invocations, nor are they directly
    invoking ActorAbilities. They're just telling Sloth how it should attack*/

    //the event that will be invoked if the player stays too still for too long
    [NonSerialized]
    public UnityEvent playerStandStill = null;

    //the event that will be invoked if the player moves
    [NonSerialized]
    public UnityEvent playerMove = null;

    //the player's rigidbody
    private Rigidbody2D rb2d;

    //the amount of time the player has been moving
    private float internalDelay = 0.0f;

    //METHODS--------------------------------------------------------------------------------------
    //called the first frame this component is active
    void Start()
    {
        //gets a reference to the player's rigidbody
        rb2d = this.gameObject.GetComponent<Rigidbody2D>();

        //turns on the coroutine that times melee attacks
        StartCoroutine(ProcessMelee());
    }

    //should the player die, this component should cease to exist
    public void DoActorDeath()
    {
        Destroy(this);
    }

    //reads movement input
    public void OnMovement()
    {
        //invoke an attack that occurs on player movement
        playerMove.Invoke();
    }

    /*runs the logic for melee
    yes, we are checking the value and storing it every frame, but checking the
    physics sim is more efficient than comparing vectors (which can sometimes not
    compare correctly).
    Technically can result in a melee attack triggering if the player gets grabbed right
    before the player triggers a melee, because getting dragged into a marker counts on
    moving, but I (Thomas) don't anticipate this being a big deal.*/
    IEnumerator ProcessMelee()
    {
        //in theory this coroutine should run until the player dies
        while(true)
        {
            //Debug.Log($"melee timer at {internalDelay}");
            if(rb2d.IsSleeping())
            {
                internalDelay = 0.0f;
            }
            else
            {
                internalDelay += Time.deltaTime;
            }

            if(internalDelay >= attackDelay)
            {
                internalDelay = 0.0f;
                playerStandStill.Invoke();
            }

            yield return null;
        }
    }
}
