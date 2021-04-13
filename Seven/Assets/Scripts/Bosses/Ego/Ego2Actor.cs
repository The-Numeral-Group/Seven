using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Ego's Second Phase is so different from its first that it requires two seperate Actor
classes to function.*/

public class Ego2Actor : Actor
{
    /*How ego works:
    Do a Teleport Rush
        Teleport to a random place in a range 3 times
        Teleport a 4th time to a random position near the player
    Attack
    Wait a little bit
    Repeat

    Simple, right?
    */

    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("The mesh in which Ego's teleports will randomly arrive in.")]
    public GameObject teleMesh;

    [Tooltip("How many times Ego should teleport before the final attack teleport.")]
    public int teleCount = 3;

    [Tooltip("How long Ego should wait after a teleport completes before teleporting again.")]
    public float teleWait = 0.5f;

    [Tooltip("How long Ego should wait after an attack completes before attacking again.")]
    public float attackWait = 0.5f;

    [Tooltip("The minimum distance Ego should be from the player after the last teleport.")]
    public float teleMinDist = 1f;

    [Tooltip("The maximum distance Ego should be from the player after the last teleport.")]
    public float teleMaxDist = 5f;

    [Tooltip("Controls how many times Ego should use normal attacks before using its special.")]
    public int specialAttackGate = 7;

    [Tooltip("Controls how many times Ego should use its beam attack before using its" +
        " wall attack.")]
    public int powerAttackGate = 3;

    //How many times Ego has attacked (which determines which attack it uses). Needs to start at
    //one to work with the modulo-based attack determination
    private int attackCount = 1;

    //whatever ability Ego is currently using, if any
    private ActorAbility currAbility;

    //private a re-do reference to Ego's movement, but casted to its unique type
    private Ego2Movement uniqueMovement;

    //writable reference to this for ability invocations
    private Actor ego;

    //reference to player for ease
    private GameObject player;

    //reference to the teleMesh's literal mesh for ease
    private Mesh tMesh;

    //METHODS--------------------------------------------------------------------------------------
    // Start calls the first frame this gameObject is active
    new void Start()
    {
        //do normal initialization
        base.Start();

        //get the player
        player = GameObject.FindGameObjectWithTag("Player");

        //get writable reference to this
        ego = this.gameObject.GetComponent<Actor>();

        //get teleMesh's mesh
        tMesh = teleMesh.GetComponent<MeshFilter>().mesh;

        //get ActorMovement as Ego2Movement
        if(this.myMovement is Ego2Movement)
        {
            uniqueMovement = (this.myMovement as Ego2Movement);
        }
        else
        {
            Debug.LogError("Ego2Actor: No Ego2Movement found, teleportations will not function");
        }

        //start the behaviour coroutine
        StartCoroutine(BossBehaviour());
    }

    // Controls the timing of Ego's attacks and teleportations
    IEnumerator BossBehaviour()
    {
        while(true)
        {
            //Step 1: Random Teleports
            yield return TeleportRush();

            //Step 2: Attack
            ExecuteAttack();

            //Step 3: Wait for the attack to resolve
            yield return new WaitUntil( () =>  (currAbility && !currAbility.getIsFinished()) );

            //Step 4: Wait a little while
            yield return new WaitForSeconds(attackWait);
        }
    }

    //executes teleCount many random teleports, then teleports near the player
    IEnumerator TeleportRush()
    {
        //represents the area of the teleMesh
        Bounds meshBound = tMesh.bounds;

        for(int i = 0; i < teleCount; ++i)
        {
            //Ego will teleport to a random position within the mesh's area
            var randomDestinationVec = new Vector3(
                //Random.Range(meshBound.min.x, meshBound.max.x) * meshBound.size.x,
                Random.Range(-1f, 1f) * meshBound.size.x,
                //Random.Range(meshBound.min.y, meshBound.max.y) * meshBound.size.y,
                Random.Range(-1f, 1f) * meshBound.size.y,
                0f
            );
            yield return uniqueMovement?.EgoTeleport(randomDestinationVec);

            //wait a little bit before doing the next teleport
            yield return new WaitForSeconds(teleWait);
        }

        //set destination to whatever the player is but a little bit above and to the right
        //we can make it be a full 360 later
        var playerDestinationVec = player.transform.position + new Vector3(
            Random.Range(teleMinDist, teleMaxDist),
            Random.Range(teleMinDist, teleMaxDist),
            0f
        );
        yield return uniqueMovement?.EgoTeleport(playerDestinationVec);
    }

    /*executes an attack based on modulos of attacks. If attackCount % X is 0, then it must be
    the Xth attack made by Ego. The attack priority is Special < Wall < Beam*/
    void ExecuteAttack()
    {
        ActorAbility attack;

        if(attackCount % specialAttackGate == 0)
        {
            //do special
            attack = this.myAbilityInitiator.abilities[AbilityRegister.EGO_SUMMON];
        }
        else if(attackCount % powerAttackGate == 0)
        {
            //do power
            attack = this.myAbilityInitiator.abilities[AbilityRegister.EGO_WALL];
        }
        else
        {
            //do wall
            attack = this.myAbilityInitiator.abilities[AbilityRegister.EGO_BEAM];
        }

        currAbility = attack;
        attack.Invoke(ref ego, player);
        ++attackCount;
    }
}
