using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Doc: https://docs.google.com/document/d/1xce8XIn-Tmzo_cYJCvutIhglOUQfMZZNBiaEYop-oxo/edit
public class GhostKnightSpecial : ActorAbilityFunction<Actor, int>
{
    //How long this entire process should take.
    // duration = duration_vanish_start + duration_vanish + duration_appear + 2 * duration_slash
    public float duration;
    //How long the vanising start process should take.
    public float duration_vanish_start;
    //How long the ghost knight should be invisible for.
    public float duration_vanish;
    //How long the reappearing process should take.
    public float duration_appear;

    // Duration of slash moves after appearing
    public float duration_slash;

    private Actor player;

    public GameObject glintObject;

    GhostKnightAnimationHandler ghostKnightAnimationHandler;

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
            this.duration = 9f;
        }

        ghostKnightAnimationHandler = args[0].myAnimationHandler as GhostKnightAnimationHandler;

        StartCoroutine(args[0].myMovement.LockActorMovement(duration));
        StartCoroutine(Vanish(args[0]));
        return 0;
    }
    private IEnumerator Vanish(Actor user)
    {
        SpriteRenderer gkSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        user.mySoundManager.PlaySound("SpecialVanish");

        // Set both actors to be invincible.
        user.myHealth.SetVulnerable(false, -1);

        float opacity = 1f;
        while (opacity > 0f)
        {
            opacity -= 0.1f ;
            gkSpriteRenderer.color = new Color(1f, 1f, 1f, opacity);
            yield return new WaitForSeconds(this.duration_vanish_start / 10);
        }
        yield return new WaitForSeconds(this.duration_vanish * (3/4));
        Teleport(user);
    }
    private void Teleport(Actor user)
    {
        Vector2 playerPos = player.transform.position;

        playerPos.y += 5;

        user.transform.position = playerPos;
        StartCoroutine(Reappear(user));
    }
    private IEnumerator Reappear(Actor user)
    {
        var glint = Instantiate(this.glintObject, user.transform.position, Quaternion.identity);

        // Perform VSlash while appearing.
        StartCoroutine(PerformSpecialSlash(user));

        // Play glint audio
        user.mySoundManager.PlaySound("SpecialEyeGlint");

        yield return new WaitForSeconds(this.duration_vanish / 4);

        Destroy(glint);

        SpriteRenderer gkSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();

        float opacity = 0f;
        while (opacity < 1f)
        {
            opacity += 0.1f;
            gkSpriteRenderer.color = new Color(1f, 1f, 1f, opacity);
            yield return new WaitForSeconds(this.duration_appear / 10);
        }

        // Set both actors to be no longer invincible.
        user.myHealth.SetVulnerable(true, -1);

    }

    private IEnumerator PerformSpecialSlash(Actor user)
    {
        ghostKnightAnimationHandler.animateSpecialSlash();
        user.myMovement.DragActor(new Vector2(0.0f, -1f));
        yield return new WaitForSeconds(this.duration_slash);
        user.myMovement.DragActor(new Vector2(0.0f, 0.0f));
        isFinished = true;
    }

}
