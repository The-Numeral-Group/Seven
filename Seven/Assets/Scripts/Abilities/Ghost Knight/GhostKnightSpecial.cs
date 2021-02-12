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

    // Ghost Knight Effector object
    public GameObject gkEffector;

    public GameObject glintObject;

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


        // Set both actors to be invincible.
        user.myHealth.vulnerable = false;

        // Turn off effector so player doesn't get knockback when Ghost Knight is invisible.
        gkEffector.SetActive(false);

        float opacity = 1f;
        while (opacity > 0f)
        {
            opacity -= 0.1f ;
            gkSpriteRenderer.color = new Color(1f, 1f, 1f, opacity);
            yield return new WaitForSeconds(this.duration_vanish_start / 10);
        }
        yield return new WaitForSeconds(this.duration_vanish/2);
        Teleport(user);
    }
    private void Teleport(Actor user)
    {
        user.transform.position = player.transform.position;
        StartCoroutine(Reappear(user));
    }
    private IEnumerator Reappear(Actor user)
    {
        Instantiate(this.glintObject, user.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(this.duration_vanish / 2);

        SpriteRenderer gkSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();

        float opacity = 0f;
        while (opacity < 1f)
        {
            opacity += 0.1f;
            gkSpriteRenderer.color = new Color(1f, 1f, 1f, opacity);
            yield return new WaitForSeconds(this.duration_appear / 10);
        }


        // Set both actors to be no longer invincible.
        user.myHealth.vulnerable = true;

        // Turn back the effector on.
        gkEffector.SetActive(true);

        isFinished = true;
    }
}
