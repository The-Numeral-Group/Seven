using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathEnraged : ActorAbilityFunction<Actor, int>
{
    // How long the player will get pushed away
    public float pushDuration;
    
    // Rate of how hard the player will get pushed away
    public float pushIntensity;

    public string animTrigger;

    private bool isPushing;
    private Actor player;

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
        FinishEnraged();
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(pushDuration);
        isPushing = false;
    }

    private void FinishEnraged()
    {
        // Turn off wrath's invincibility
        user.myHealth.SetVulnerable(true, -1f);

        this.isFinished = true;
    }
}
