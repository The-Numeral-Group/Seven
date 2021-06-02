using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeliefOfInvincibility : ActorAbilityCoroutine<int>
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("The amount of times the user can activate the effect per room.")]
    public int usesPerFight = 2;

    [Tooltip("The amount of time the user MUST remain locked in place.")]
    public float minLockTime = 1f;

    //The amount of uses this ability has left for this room
    private int usesRemaining = 0;

    //The user's original RB2D constraints, if any
    private RigidbodyConstraints2D constraints = RigidbodyConstraints2D.None;

    //Whether or not the user has tried to attack recently
    private bool recentAttack = false;

    //METHODS--------------------------------------------------------------------------------------
    // initialize remaining uses before fight starts
    void Awake()
    {
        usesRemaining = usesPerFight;
    }
    protected override IEnumerator InternCoroutine(params int[] args)
    {
        //immediately exit if there are no uses remaining
        /*if(usesRemaining == 0)
        {
            yield break;
        }*/

        //Step 0: Make the user invulnerable
        this.user.myHealth.SetVulnerable(false, -1f);

        //Step 1: Stop the user in their tracks
        this.user.myMovement.MoveActor(Vector2.zero);

        //Step 2.1: Save the user's movement constraints
        constraints = this.user.myMovement.rigidbody.constraints;

        //Step 2.2: Lock the user in place
        this.user.myMovement.rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;

        /*The user is no longer able to move at all. Nothing can push them or rotate them.
        Animations might still update, but that'll be ammended when there are animations.
        Or... maybe it shouldn't be? I (Thomas) am gonna test that.*/

        //Step 3: wait a bit
        yield return new WaitForSeconds(minLockTime);

        /*Anyways, despite being locked in place, the user's ActorMovement can still read
        inputs/move directives. This will case movementDirection to move from Vector2.zero to 
        something else, indicating the user's attempt to move. So, the ability will end.
        This is part of the reason why constraints are used instead of LockActorMovement:
        so that movementDirection can keep updating*/

        //Step 4: wait until the user tries to move
        yield return new WaitUntil ( 
            () => this.user.myMovement.movementDirection != Vector2.zero 
        );

        //Step 5: Unlock the user's constraints
        this.user.myMovement.rigidbody.constraints = constraints;

        //Step 6: Make the user vulnerable again
        this.user.myHealth.SetVulnerable(true, -1f);

        //And now we're done!

    }

    /*these four methods will intercept the user's attacks and end the ability should an attack be
    detected. Really only works for the player, there isn't a super easy way to read for when 
    abilities are used, let alone read ones that are specifically attacks.*/
    public void OnAttack()
    {
        StartCoroutine(AttackBuffer());
    }

    public void OnAbilityOne()
    {
        StartCoroutine(AttackBuffer());
    }

    public void OnAbilityTwo()
    {
        StartCoroutine(AttackBuffer());
    }

    IEnumerator AttackBuffer()
    {
        recentAttack = true;

        //Debug.Log("BeliefOfInvincibility: attack read");

        yield return new WaitForSeconds(0.5f);

        recentAttack = false;
    }
}
