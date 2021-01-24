using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostKnightKnockBack : ActorAbilityFunction<Actor, int>
{
    //How long the player will be stunned for.
    public float stunned_duration = 0.05f;

    private Actor player;

    public override void Invoke(ref Actor user)
    {
        if (usable)
        {
            var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];
            player = playerObject.GetComponent<Actor>();

            InternInvoke(user);
            StartCoroutine(coolDown(0.0f));
        }
    }

    protected override int InternInvoke(params Actor[] args)
    {
        StartCoroutine(player.myMovement.LockActorMovement(stunned_duration));
        StartCoroutine(pushPlayer(args[0]));
        return 0;
    }

    private IEnumerator pushPlayer(Actor user)
    {
        player.myMovement.DragActor(user.myMovement.movementDirection * 5);
        player.myMovement.force = 0.15f;
        yield return new WaitForSeconds(stunned_duration);
        player.myMovement.DragActor(Vector2.zero);
        player.myMovement.force = Time.deltaTime;
    }
}
