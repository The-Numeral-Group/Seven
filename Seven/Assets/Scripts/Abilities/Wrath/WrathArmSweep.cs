using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Wraths sweep ability. Animation handles enabling and disabling of hitbox
public class WrathArmSweep : WeaponAbility
{
    // How long the player will be stunned for after getting hit.
    public float stunnedDuration;

    public GameObject alreadyInstantiatedWeaponObject;
    protected override void Start()
    {
        weaponObject = alreadyInstantiatedWeaponObject;
        weaponObject.transform.parent = this.gameObject.transform;
        weaponObject.SetActive(false);

        //set damage of hitboxes
        if (damagePerHitbox.Count > 0)
        { 
            SetDamage(damagePerHitbox[0]);
        }
        else
        {
            SetDamage(0);
        }
    }

    protected override int InternInvoke(params Actor[] args)
    {
        this.hitConnected = false;
        if(animTrigger.Length != 0)
        {
            user.myAnimationHandler.TrySetTrigger(animTrigger);
        }
        StartCoroutine(CheckIfAnimFinished());
        return 0;
    }

    IEnumerator CheckIfAnimFinished()
    {
        while(this.user.myAnimationHandler.IsInState(animTrigger))
        {
            yield return new WaitForFixedUpdate();
        }
        FinishArmSweep();
    }

    protected virtual void FinishArmSweep()
    {
        weaponObject.SetActive(false);
        this.isFinished = true;
    }

    public IEnumerator stunPlayer(float dragBackDuration)
    {
        yield return new WaitForSeconds(dragBackDuration);

        var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];
        playerObject.GetComponent<Actor>().myMovement.DragActor(Vector2.zero);
        playerObject.GetComponent<Actor>().myMovement.MoveActor(Vector2.zero);

        // Stop player's animation
        playerObject.GetComponent<Animator>().SetBool("player_walking", false);

        // Stop player's sound
        playerObject.GetComponent<ActorSoundManager>().StopSound("PlayerRun");

        // Lock player's movement animation and sound
        playerObject.GetComponent<PlayerMovement>().canMove = false;

        // Lock player's movement
        StartCoroutine(playerObject.GetComponent<Actor>().myMovement.LockActorMovement(this.stunnedDuration));

        // Turn off player's dodge ability.
        playerObject.GetComponent<PlayerAbilityInitiator>().canDodge = false;

        yield return new WaitForSeconds(this.stunnedDuration);

        // Turn on player's dodge ability.
        playerObject.GetComponent<PlayerAbilityInitiator>().canDodge = true;

        // Turn on player's movement animation and sound
        playerObject.GetComponent<PlayerMovement>().canMove = true;
    }
}
