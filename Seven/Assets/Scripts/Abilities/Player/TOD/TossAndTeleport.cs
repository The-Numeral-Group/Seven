using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TossAndTeleport : ProjectileAbility
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("How far the sword should travel before stopping.")]
    public float travelDistance = 5f;

    [Tooltip("How much damage the sword should deal on impact.")]
    public float damage = 2f;

    [Tooltip("How many times the user should be able to activate this ability in one scene.")]
    public int maxUses = 5;

    //middleman variable for remembering the last sword that was thrown
    private GameObject currentSword;

    //whether or not there is a currently deployed sword
    private bool swordOut = false;

    //amount of remaining uses
    private int usesRemaining;

    //internal user reference typed to player
    private PlayerActor internalUser;

    //METHODS--------------------------------------------------------------------------------------
    // Awake is called before everything starts
    void Awake()
    {
        usesRemaining = maxUses;
    }

    /*Launch the projectile. The anticipated argument is the gameObject being shot at. The 
    gameObject may-or-may-not have an ActorHealth component. However, the sword for 
    TossAndTeleport is always tossed in the direction of the faceAnchor.*/
    protected override int InternInvoke(params Vector2[] args)
    {
        //cast the user as a PlayerActor to gain access to the needed sword state methods
        if(user is PlayerActor)
        {
            internalUser = (user as PlayerActor);
        }
        else
        {
            Debug.LogError("TossAndTeleport: This ability can only be used by the player!");
            return 1;
        }

        //auto return if the user it out of invokations for this ability
        if(usesRemaining <= 0)
        {
            return 1;
        }

        usable = false;
        StartCoroutine(SwordBehaviour());
        return 0;
    }

    //Coroutine exists only to time the teleport correctly relative to everything else
    //because you know the teleport still takes a little while
    IEnumerator SwordBehaviour()
    {
        if(swordOut)
        {
            yield return StartCoroutine(TeleportToSword());
        }
        else
        {
            ThrowSword();
            yield return null;
        }

        swordOut = true;
        isFinished = true;
    }

    //Launches the sword projectile and disarms the user, if possible.
    void ThrowSword()
    {
        //the sword is always thrown in direction of the user's face anchor
        Vector3 directionToFace = 
            (internalUser.faceAnchor.position 
                - internalUser.gameObject.transform.position).normalized;

        //The component is added on and then launched in the direction of the faceAnchor
        projObj.AddComponent<PlayerSwordProjectile>().Launch(
            directionToFace, 
            LAUNCH_MODE.DIRECTION,
            this
        );

        //put user in the swordless state
        internalUser.SetSwordState(false);

        currentSword = projObj;
    }

    //If there's a sword, EgoTeleport the user to it and reequip it
    IEnumerator TeleportToSword()
    {
        if(currentSword)
        {
            yield return StartCoroutine(
                Ego2Movement.EgoTeleport(currentSword.transform.position, internalUser.gameObject)
            );
        }
        else
        {
            Debug.LogError("TossAndTeleport: no sword to teleport to!");
        }

        //Tell the sword that it's been picked back up
        //Actually don't do that
        //ReEquip();
        
        //Destory projObj to prevent redundant sword creation
        Destroy(projObj);

        

        yield return null;
    }

    //Cleans up sword after it has been picked up
    public void ReEquipSword()
    {
        swordOut = false;

        Destroy(currentSword);

        //put user in unswordless state
        internalUser.SetSwordState(true);
    }
}

internal class PlayerSwordProjectile : BasicProjectile
{
    //FIELDS---------------------------------------------------------------------------------------
    //how far the sword should travel before stopping
    private float travelDistance = 5f;

    //the sword's starting point for judging distance calculations
    private Vector3 startingPoint;

    //the wrapper, in case the sword needs to tell the wrapper that it has been picked up
    private TossAndTeleport wrapper;

    //METHODS--------------------------------------------------------------------------------------
    //Does the normal launch with a default distance of 5 and damage of 2
    public override void Launch(Vector2 target, LAUNCH_MODE mode = LAUNCH_MODE.POINT)
    {
        travelDistance = 5f;
        damage = 2f;
        startingPoint = this.gameObject.transform.position; 

        base.Launch(target, mode);
    }

    /*Starts the projectile! Hides the OG launch in exchange for a user defined travel distance*
    and damage value*/
    public void Launch(Vector2 target, LAUNCH_MODE mode = LAUNCH_MODE.POINT, 
        TossAndTeleport wrapper=null)
    {
        this.travelDistance = wrapper.travelDistance;
        this.damage = wrapper.damage;  
        this.wrapper = wrapper;
        startingPoint = this.gameObject.transform.position;

        base.Launch(target, mode);
    }

    /*What should happen every time the projectile moves (including the movement)*/
    protected override void InternalMovement(Vector2 movementDirection)
    {
        //first, move the sword
        mover.MoveActor(movementDirection);

        //Then calculate how far it has gone
        var distFromHome = Mathf.Abs(
            Vector3.Distance(
                startingPoint, 
                this.gameObject.transform.position
            )
        );

        //if it has gone too far, it should stop moving
        if(distFromHome >= travelDistance)
        {
            this.moveFunction = null;
        }
    }

    //What happens when the projectile actually hits something
    protected override void OnTriggerEnter2D(Collider2D collided)
    {
        //if it's the player, make them pick up the sword
        if(collided.gameObject.CompareTag("Player"))
        {
            wrapper?.ReEquipSword();
        }
        //if not, just do the regular hit
        else
        {
            base.OnTriggerEnter2D(collided);
        }

    }
}
