using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
GhostKnightActor's main function is to run the State Machine that powers
Ghost Knight's boss fight
Doc: https://docs.google.com/document/d/1gMAcVBLBTMIsFyGU-nBVzXPtR3GNvvGYiVKzjK3un20/edit
*/
public class GhostKnightActor : Actor
{

    [Tooltip("Indicates which nth attack Ghost Knight performs the special attack")]
    public int specialAttackGate = 7;

    [Tooltip("How close the player should be before Ghost Knight tries to slash them")]
    public float slashRange = 25f;

    [Tooltip("Chance of picking projectile attack compared to slash attack. (1 = 1:1, 2 = 1:2, and so on)")]
    public float projectileRatio = 3f;

    [Tooltip("Ghost Knight's physical contact damage")]
    public float damage = 1f;

    [Tooltip("Delay before Ghost Knight starts attacking")]
    public float introDelay = 1f;

    public Canvas attackTutorialCanvas;

    public GameObject gameSaveManager;

    private bool attackEnabled = false;

    private int specialAttackCounter = 1;
    private int attackTutorialCounter = 2;

    private Actor ghostKnight;
    private Actor player;

    public State currentState;
    private ActorAbility currAbility;

    private ActorAbility slash;
    private ActorAbility proj;
    private ActorAbility special;

    public enum State
    {
        WAITING,
        WALK,
        PHYSICAL_SLASH,
        LAUNCH_PROJECTILE,
        SPECIAL,
        NULL,
    }


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];

        if (playerObject == null)
        {
            Debug.LogWarning("GhostKnightActor: Ghost Knight can't find the player!");
        }
        else
        {
            player = playerObject.GetComponent<Actor>();
        }

        slash = this.myAbilityInitiator.abilities[AbilityRegister.GHOSTKNIGHT_SLASH];
        proj = this.myAbilityInitiator.abilities[AbilityRegister.GHOSTKNIGHT_PROJECTILE];
        special = this.myAbilityInitiator.abilities[AbilityRegister.GHOSTKNIGHT_SPECIAL];

        ghostKnight = this.gameObject.GetComponent<GhostKnightActor>();
        currentState = State.WALK;
        currAbility = null;

        StartCoroutine(introDelayStart());
    }

    //Function that is called when GhostKnight dies. Starts the next cutscene.
    public override void DoActorDeath()
    {
        var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];
        this.gameSaveManager.GetComponent<GameSaveManager>().setVectorValue(playerObject.transform.position, 2);
        this.gameSaveManager.GetComponent<GameSaveManager>().setVectorValue(transform.position, 3);
        SceneManager.LoadScene("Tutorial_PostFight");
    }

    // When the game starts, the ghost knight will try to cast any attack with movementDirection
    // of vector zero. This will cause some issue with knockback, casting abilities that require position.
    // To avoid this, I have added a short delay before the ghost knight attacks. 
    // When the game starts, the ghost knight will walk for short time then try to attack. 
    private IEnumerator introDelayStart()
    {
        EvaluateState(currentState);
        yield return new WaitForSeconds(this.introDelay);
        this.attackEnabled = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (attackEnabled)
        {
            if (currAbility != null)
            {
                checkIfAbilityDone();
            }
            if ((currentState == State.WAITING) || (currentState == State.WALK))
            {
                decideNextState();
                EvaluateState(currentState);
            }
        }
        if(myHealth.currentHealth == 0)
        {
            System.Tuple<Actor, System.Action<Actor>> ghostKnightIdle =
                new System.Tuple<Actor, System.Action<Actor>>(ghostKnight, null);
            gameObject.SendMessage("NextPhase", ghostKnightIdle);
        }
    }

    void EvaluateState(State state)
    {
        switch (state)
        {
            case State.WALK:
                stepTowardsPlayer();
                break;

            case State.PHYSICAL_SLASH:

                slash.Invoke(ref ghostKnight);
                specialAttackCounter++;

                break;

            case State.LAUNCH_PROJECTILE:

                proj.Invoke(ref ghostKnight);
                specialAttackCounter++;

                break;

            case State.SPECIAL:

                special.Invoke(ref ghostKnight);

                break;

            case State.NULL:
                break;

            default:
                Debug.LogWarning("GhostKnightActor: Ghost Knight has fallen out of its state machine!");
                break;
        }
    }
    void stepTowardsPlayer()
    {
        var myPos = this.gameObject.transform.position;
        var playerPos = player.gameObject.transform.position;

        playerPos.y += 4;

        // This prevents GK from swerving when player is not moving and GK is on top of the player
        if (player.myMovement.movementDirection == Vector2.zero)
        { 
            if((myPos.x < (playerPos.x + 0.2)) && (myPos.x > (playerPos.x - 0.2)) 
                && (myPos.y < (playerPos.y + 0.2)) && (myPos.y > (playerPos.y - 0.2)))
            {
                Debug.Log("STOP MOVING");
                this.myMovement.MoveActor(Vector2.zero);
                return;
            }
        }

        var directionToPlayer = (playerPos - myPos).normalized;

        this.myMovement.MoveActor(directionToPlayer);

    }
    void checkIfAbilityDone()
    {
        // If the currAbility has finished, reset.
        if (currAbility.getIsFinished())
        {
            currAbility = null;
            currentState = State.WAITING;
        }
    }
    void decideNextState()
    {
        var distanceToPlayer = Vector2.Distance(player.transform.position, this.gameObject.transform.position);

        bool slashReady = slash.getUsable(), projReady = proj.getUsable(), specialReady = special.getUsable();

        // Determines which attack the ghost knight will perform.
        int whichAtt = (int)Random.Range(1, projectileRatio + 2);


        // check for special attack counter.
        // if it is 7, activate special attack. 
        if ((specialAttackCounter >= specialAttackGate) && specialReady)
        {
            specialAttackCounter = 1;
            currentState = State.SPECIAL;
            currAbility = special;
        }
        else if ((whichAtt == 1) && projReady)
        {
            currentState = State.LAUNCH_PROJECTILE;
            currAbility = proj;
        }
        else if ((whichAtt != 1) && (distanceToPlayer <= slashRange) && slashReady)
        {
            currentState = State.PHYSICAL_SLASH;
            currAbility = slash;
        }
        else
        {
            currentState = State.WALK;
        }

    }
    
    
    // Handles the physical contact with the player effect.
    // Knockback Effect is handled with Point Effector 2D Component.
    /*void OnTriggerEnter2D(Collider2D collider)
    {
        // Only collide with player
        if (collider.gameObject.tag == "Player")
        {

            var playerHealth = collider.gameObject.GetComponent<ActorHealth>();

            //or a weakpoint if there's no regular health
            if (playerHealth == null) { collider.gameObject.GetComponent<ActorWeakPoint>(); }

            //if the enemy can take damage (if it has an ActorHealth component),
            //hurt them. Do nothing if they can't take damage.
            if (playerHealth != null)
            {
                if (!playerHealth.vulnerable)
                {
                    return;
                }
                playerHealth.takeDamage(damage);
            }
        }
    }*/

    public override void DoActorDamageEffect(float damage)
    {
        base.DoActorDamageEffect(damage);
        // Play TakeDamage Audio
        mySoundManager.PlaySound("TakeDamage");
        // Take away counter for AttackTutorialText
        this.attackTutorialCounter--;
        if(this.attackTutorialCounter == 0)
        {
            this.attackTutorialCanvas.enabled = false;
        }
    }
}
