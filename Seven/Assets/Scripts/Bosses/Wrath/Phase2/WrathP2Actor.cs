using System.Collections;
using UnityEngine;

public class WrathP2Actor : Actor
{
    //Static bool used to determine if the target is in range for the melee attack.
    public static bool targetInRange;
    //Static float used to determine if the speed at which wrath's abilities get executed.
    //Meant to be utilized by the abilities.
    public static float abilitySpeedMultiplier {get; private set;}
    //Static float used to as additional damage for actor abilities
    public static int abilityDamageAddition {get; private set;}
    //Affects the delay between ability casts
    [Tooltip("The amount of time Wrath waits between attacks.")]
    [Range(0f, 10f)]
    public float delayBetweenAttacks = 1f;
    //At which changes in percentage differenced do we change phases. Used to increment the phasechangeactual value
    [Tooltip("At which percentage differences do we change phases? I.e. if .25, the boss will change phases every 25%.")]
    [Range(0.1f, 0.5f)]
    public float phaseChangePercentageInspector = .25f;
    //Reference to the current state
    [Tooltip("The current state the actor is in. Meant to just be inspector viewable. Changint the value in inspector will do nothing.")]
    public State currState;
    public enum State
    {
        WAITING,
        DEAD,
        PHYSICAL,
        SHOCKWAVE,
        FIREBRIMSTONE
    }
    //Rerference to the statemachine coroutine
    IEnumerator StateMachinePTR;
    //Reference to this instance of the class
    Actor self;
    //Reference to the target
    Actor target;
    //Reference to the currentAbility
    ActorAbility currAbility;
    //flag to let the actor know they have 'died'
    bool isDead;
    //Initialized by the inspector version. I set it up so that we avoid situations where editing the phase change value in inspector messes something up.
    float phaseChangePercentageActual;

    //Initialize member variables
    void Awake()
    {
        self = this;
        WrathP2Actor.targetInRange = false;
        WrathP2Actor.abilitySpeedMultiplier = 1f;
        WrathP2Actor.abilityDamageAddition = 0;
        isDead = false;
        phaseChangePercentageActual = phaseChangePercentageInspector;
        currAbility = null;
    }

    //Setup reference to the target
    protected override void Start()
    {
        base.Start();
        SetupTarget();
        //Start the state machine
        StateMachinePTR = StateMachine();
        StartCoroutine(StateMachinePTR);
    }

    void FixedUpdate()
    {

    }

    //Do Actor death function
    public override void DoActorDeath()
    {
        base.DoActorDeath();
        //We set isdead so that the state machine no longer reloops on its next evaluation.
        isDead = true;
    }

    //Function is used to setup the reference to whomever wrath is targeting. i.e. the player
    public void SetupTarget()
    {
        if (target == null)
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.GetComponent<Actor>();
            }
            else
            {
                Debug.LogWarning("IndulgenceP1Actor: unable to find target for indulgence.");
            } 
        }
    }

    //Determine what the actors next state will be. Treating it like a behaviour tree of if else's
    //EvaluateState is called from the statemachine coroutine.
    void EvalauteState()
    {
        // Chooses either Shockwave or Fire&Brimstone
        //int abilityType = (int)Random.Range(0, 2);
        int abilityType = 1;  // FOR TESTING FIRE BRIM

        Debug.Log("Evaluating State");
        State decidingState = currState;
        if(isDead) //Death is used to kill the state machine.
        {
            decidingState = State.DEAD;
            ExecuteState(decidingState);
        }
        else if (WrathP2Actor.targetInRange && this.myAbilityInitiator.abilities[AbilityRegister.WRATH_ARMSWEEP].getUsable())
        {
            decidingState = State.PHYSICAL;
        }
        else if (abilityType == 0 && this.myAbilityInitiator.abilities[AbilityRegister.WRATH_SHOCKWAVE].getUsable())
        {
            decidingState = State.SHOCKWAVE;
        }
        else if (abilityType == 1 && this.myAbilityInitiator.abilities[AbilityRegister.WRATH_FIREBRIMSTONE].getUsable())
        {
            decidingState = State.FIREBRIMSTONE;
        }
        else
        {
            decidingState = State.WAITING;
        }
        /*After checking the other states we do a health evaluation.
        We perform this just in case we want an ability to happen on phase change
        It is separate from the if-else chain above so that it can override w/e state decision
        was made by the if-else checks.*/
        if(EvaluateHealth())
        {
            //some state change
        }
        ExecuteState(decidingState);
    }

    //Called from evaluate state
    void ExecuteState(State state)
    {
        switch(state)
        {
            case State.PHYSICAL:
                Debug.Log("Choosing arm sweep");
                currState = State.PHYSICAL;
                currAbility = this.myAbilityInitiator.abilities[AbilityRegister.WRATH_ARMSWEEP];
                currAbility.Invoke(ref self);
                break;
            case State.SHOCKWAVE:
                Debug.Log("Choosing Shockwave");
                currState = State.SHOCKWAVE;
                currAbility = this.myAbilityInitiator.abilities[AbilityRegister.WRATH_SHOCKWAVE];
                currAbility.Invoke(ref self);
                break;
            case State.FIREBRIMSTONE:
                Debug.Log("Choosing Fire and Brimstone");
                currState = State.FIREBRIMSTONE;
                currAbility = this.myAbilityInitiator.abilities[AbilityRegister.WRATH_FIREBRIMSTONE];
                currAbility.Invoke(ref self);
                break;
            case State.WAITING:
                currState = State.WAITING;
                currAbility = null;
                break;
            case State.DEAD: //Dead state is used to kill the state machine
                currState = State.DEAD;
                currAbility = null;
                return;
            default:
                Debug.Log("Wrathp2Actor: No longer inside the state machine.");
                break;
        }
        //rerun the state machine
        StateMachinePTR = StateMachine();
        StartCoroutine(StateMachinePTR);
    }

    //Used to determine the phases we are in for the wrath p2 fight.
    //Called from within evaluate state.
    //returns true pn phase change, false otherwise.
    //Can produce unintended results it phasechangePercentageInspector is changed at runtime.
    bool EvaluateHealth()
    {
        if (this.myHealth.currentHealth < this.myHealth.maxHealth * (1-phaseChangePercentageActual))
        {
            //every 2 phase changes we bump up the damage
            int damageValue = ((int)(phaseChangePercentageActual / phaseChangePercentageInspector));
            //Increase damage every two phase changes
            if (damageValue % 2 == 0)
            {
                WrathP2Actor.abilityDamageAddition = damageValue / 2;
                WrathArmSweep armSweep = this.myAbilityInitiator.abilities[AbilityRegister.WRATH_ARMSWEEP] as WrathArmSweep;
                armSweep.AddDamage(WrathP2Actor.abilityDamageAddition/WrathP2Actor.abilityDamageAddition);
            }
            //Every phase change we increase the speed
            WrathP2Actor.abilitySpeedMultiplier += 0.5f;
            this.myAnimationHandler.Animator.SetFloat("anim_speed", WrathP2Actor.abilitySpeedMultiplier);
            //We increase what the value for which the next phase should be evaluated at.
            phaseChangePercentageActual += phaseChangePercentageInspector;
            return true;
        }
        return false;
    }

    /*Instead of using fixedupdate, I am using a coroutine to handle the statemachine. This gives me a bit of flexibility in
    controlling when things are executed*/
    IEnumerator StateMachine()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitUntil(()=> (currAbility == null || currAbility.getIsFinished()));
        if (currState != State.WAITING)
        {
            yield return new WaitForSeconds(delayBetweenAttacks);
        }
        EvalauteState();
    }
}
