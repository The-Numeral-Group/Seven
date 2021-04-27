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

        //swordOut = true;
        isFinished = true;
    }

    //Launches the sword projectile and disarms the user, if possible.
    void ThrowSword()
    {
        //the sword is always thrown in direction of the user's face anchor
        Vector3 directionToFace = 
            (internalUser.faceAnchor.position 
                - internalUser.gameObject.transform.position).normalized;

        //put user in the swordless state
        internalUser.SetSwordState(false);

        //The component is added on and then launched in the direction of the faceAnchor
        projObj.GetComponent<PlayerSwordProjectile>().Launch(
            directionToFace, 
            LAUNCH_MODE.DIRECTION,
            this
        );

        currentSword = projObj;

        swordOut = true;
    }

    //If there's a sword, EgoTeleport the user to it and reequip it
    IEnumerator TeleportToSword()
    {
        //Destory projObj to prevent redundant sword creation
        Destroy(projObj);

        if(currentSword)
        {
            //if the sword is still moving, offset the destination so that the player will end up
            //where the sword will be
            //that is to say, avoid reposting where the missile isn't
            var swordMove = currentSword.GetComponent<ActorMovement>();
            Vector3 teleDest = 
                swordMove.movementDirection == Vector2.zero ?
                currentSword.transform.position :
                currentSword.transform.position + 
                    ((Vector3)swordMove.movementDirection * swordMove.speed * 0.3f);
                //0.3f is how long a teleport takes trust me
            
            yield return StartCoroutine(
                Ego2Movement.EgoTeleport(teleDest, internalUser.gameObject)
            );
        }
        else
        {
            Debug.LogError("TossAndTeleport: no sword to teleport to!");

            //assume the sword has returned
            swordOut = false;
        }

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