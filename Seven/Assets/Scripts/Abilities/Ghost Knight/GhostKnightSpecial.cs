using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostKnightSpecial : ActorAbilityFunction<Actor, int>
{
    //How long this entire process should take.
    // duration = duration_vanish_start + duration_vanish + duration_appear
    public float duration = 5f;
    //How long the vanising start process should take.
    public float duration_vanish_start = 1f;
    //How long the ghost knight should be invisible for.
    public float duration_vanish = 3f;
    //How long the reappearing process should take.
    public float duration_appear = 1f;

    private Actor player;

    public override void Invoke(ref Actor user)
    {
        if (usable)
        {
            var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];
            player = playerObject.GetComponent<Actor>();

            isFinished = false;
            InternInvoke(user);
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    protected override int InternInvoke(params Actor[] args)
    {
        if (this.duration <= 0f)
        {
            Debug.Log("GhostKnightPhaseChange: duration must be greater than 0");
            this.duration = 2f;
        }
        StartCoroutine(args[0].myMovement.LockActorMovement(duration));
        StartCoroutine(Vanish(args[0]));
        return 0;
    }
    private IEnumerator Vanish(Actor user)
    {
        SpriteRenderer gkSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        PolygonCollider2D collider = this.gameObject.GetComponent<PolygonCollider2D>();

        // turn off collider
        collider.enabled = false;

        float opacity = 1f;
        while (opacity > 0f)
        {
            opacity -= 0.1f ;
            gkSpriteRenderer.color = new Color(1f, 1f, 1f, opacity);
            yield return new WaitForSeconds(this.duration_vanish_start / 10);
        }
        yield return new WaitForSeconds(this.duration_vanish);
        Teleport(user);
    }
    private void Teleport(Actor user)
    {
        user.transform.position = player.transform.position;
        StartCoroutine(Reappear(user));
    }
    private IEnumerator Reappear(Actor user)
    {
        SpriteRenderer gkSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        PolygonCollider2D collider = this.gameObject.GetComponent<PolygonCollider2D>();

        float opacity = 0f;
        while (opacity < 1f)
        {
            opacity += 0.1f;
            gkSpriteRenderer.color = new Color(1f, 1f, 1f, opacity);
            yield return new WaitForSeconds(this.duration_appear / 10);
        }

        // turn on collider when ghost knight is fully visible
        collider.enabled = true;

        isFinished = true;
    }
}
