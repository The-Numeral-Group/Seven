using System;
using System.Collections;
//using System.Collections.Generic;
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

    //How many attacks Sloth has done
    private int attackCount = 0;

    //whether or not sloth is actively fighting the player
    private bool activated = false;

    //player reference for easy access
    private Actor player;

    //a secondary reference to this script, because 'this' is readonly and can't be passed as a ref
    private Actor sloth;

    //an observer to watch and read the player's actions
    private SlothPlayerObserver observer;

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

        ///DEBUG
        /*sloth starts by talking with the player, but that hasn't been built yet (2/27/21), so
        it's just gonna start throwing hands immediately*/
        ActivateSloth();
        ///DEBUG
    }

    // FixedUpdate is called once per simulation tick
    void FixedUpdate()
    {
        //apparently this will make sloth face the player
        this.gameObject.transform.right = 
            player.gameObject.transform.position - this.gameObject.transform.position;
    }

    //ITS TIME
    void ActivateSloth()
    {
        //we are officially throwing hands
        activated = true;

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
            ActivateAbility(this.myAbilityInitiator.abilities[AbilityRegister.SLOTH_PHYSICAL]);
        });

        observer.playerMove = new UnityEvent();
        observer.playerMove.AddListener(delegate()
        {
            ///DEBUG
            Debug.Log("SlothActor: Sloth wants to attack because you started moving");
            ///DEBUG
            //ActivateAbility(this.myAbilityInitiator.abilities[AbilityRegister.SLOTH_RANGE]);
        });
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
                ability?.Invoke(ref sloth, player);
            }
            //if it isn't less than the limit, then it's time to TIMEWARP
            else
            {
                //haven't decided if I want the time slow to be an ability
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
    invoking ActorAbilities. They're just telling how Sloth should attack*/

    //the event that will be invoked if the player stays too still for too long
    [NonSerialized]
    public UnityEvent playerStandStill = null;

    //the event that will be invoked if the player moves
    [NonSerialized]
    public UnityEvent playerMove = null;

    //the coroutine being used to time the ranged attack
    private Coroutine timer;

    //the input action that reads how long the player has been moving
    private InputAction holdAction;

    //the delegate that invokes when the player moves for too long
    private Action holdResult;


    //might want to set this up a little differently, as OTE2D may also
    //respond to entry from other objects, like things walking into it?
    //METHODS--------------------------------------------------------------------------------------
    //called the first frame this component is active
    void Start()
    {
        /*Add a hold action to the player's movement*/
        /*holdAction = new InputAction(binding: "*{PrimaryAction}", modifiers: "hold(duration=0.4)");

        //create a delegate to invoke when the hold is done
        holdResult =  new Action<InputAction.CallbackContext>(
            (InputAction.CallbackContext c) => {OnMoveHold()}
        );

        //assign the delegate to the hold action
        holdAction.performed += holdResult;*/
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

        /*//turn on the timer for invoking the attack that occurs if the player doesn't stop moving
        if(timer != null)
        {
            StopCoroutine(timer);
        }
        timer = StartCoroutine(MovementTimer(attackDelay));*/


    }

    /*void OnMoveHold()
    {
        playerStandStill.invoke();
    }*/

    //what should happen when this object is destroyed
    void OnDestroy()
    {
        //remove the delegate
        //holdAction -= holdResult;
    }

    //waits a number of seconds, then instructs Sloth to attack the player for not standing still
    /*IEnumerator MovementTimer(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerStandStill.Invoke();
    }*/

    /*Basic Issue:
    Want to see how long the Movement Action on the player Action Map has been held
    There's a thing for that called a HoldInteraction, but I don't know how to add it 
    to an Action at runtime with code.
    Maybe there's a different way to do it?
    
    So yeah haven't done ranged stuff yet but if the events work for
    physical then it'll be fine
    
    Or, at least, the thing that goes when the player moves too long*/
    
}
