using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathEnraged : ActorAbilityFunction<Actor, int>
{
    // How long the player will get pushed away
    public float pushDuration;

    // How long Wrath will be able to spam abilities
    public float spamDuration;

    // How long Wrath will be tired out after the spam
    public float tiredDuration;
    
    // Rate of how hard the player will get pushed away
    public float pushIntensity;

    public string animTrigger;

    private bool isPushing;
    private Actor player;
    private float originalDelay;

    public override void Invoke(ref Actor user)
    {
        this.user = user;
        if (usable)
        {
            isFinished = false;
            InternInvoke(user);
        }
    }

    protected override int InternInvoke(params Actor[] args)
    {
        // Making sure the movementDirection and dragDirection have been resetted.
        user.myMovement.MoveActor(Vector2.zero);
        user.myMovement.DragActor(Vector2.zero);

        // Get player
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<Actor>();

        // Check if Wrath is already invulnerable or not. (He should be)
        if (user.myHealth.vulnerable)
        {
            Debug.LogWarning("WrathEnraged: Entering with Wrath being vulnerable");
        }

        isPushing = true;

        if (animTrigger.Length != 0)
        {
            user.myAnimationHandler.TrySetTrigger(animTrigger);
        }

        StartCoroutine(PushPlayer());

        return 0;
    }

    private IEnumerator PushPlayer()
    {
        // Start timer
        StartCoroutine(StartTimer());
        
        // Push player away    
        while(isPushing)
        {
            Vector2 pushAway = (player.gameObject.transform.position - user.gameObject.transform.position).normalized;
            player.myMovement.DragActor(pushAway * pushIntensity);
            yield return new WaitForFixedUpdate();
        }
        StartCoroutine(WrathSpamAttack());
        FinishEnraged();
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(pushDuration);
        isPushing = false;
    }

    private IEnumerator WrathSpamAttack()
    {
        originalDelay = user.GetComponent<WrathP2Actor>().delayBetweenAttacks;

        // Set delay to 0 to allow Wrath to spam attacks.
        user.GetComponent<WrathP2Actor>().delayBetweenAttacks = 0f;

        yield return new WaitForSeconds(spamDuration);

        StartCoroutine(WrathAfterSpam());
    }

    private IEnumerator WrathAfterSpam()
    {
        // Allow Wrath not to attack
        user.GetComponent<WrathP2Actor>().canAttack = false;

        // Turn off wrath's invincibility
        user.myHealth.SetVulnerable(true, -1f);

        yield return new WaitForSeconds(tiredDuration);

        // Allow Wrath to attack
        user.GetComponent<WrathP2Actor>().canAttack = true;

        // Set delay back to normal 
        user.GetComponent<WrathP2Actor>().delayBetweenAttacks = originalDelay;
    }

    private void FinishEnraged()
    {
        this.isFinished = true;
    }
}
