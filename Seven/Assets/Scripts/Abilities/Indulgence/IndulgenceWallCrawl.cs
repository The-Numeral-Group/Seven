using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IndulgenceCrush))]
public class IndulgenceWallCrawl : ActorAbilityFunction<Actor, int>
{
    IndulgenceCrush indulgenceCrush;
    Actor target;
    bool wallReached;

    void Start()
    {
        indulgenceCrush = GetComponent<IndulgenceCrush>();
    }
    public override void Invoke(ref Actor user)
    {
        Debug.LogWarning("IndulgenceWallCrawl: please use the Invoke(ref actor, param object[] args)");
    }

    public override void Invoke(ref Actor user, params object[] args)
    {
        this.user = user;
        //by default, Invoke just does InternInvoke with the provided arguments
        //it's also just implicitly convert the args and give it to InternInvoke
        if(usable)
        {
            isFinished = false;
            InternInvoke(easyArgConvert(args));
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    void SetupColliders(bool value)
    {
        var actorColliders = this.user.gameObject.GetComponents<Collider2D>();
        foreach(var actorCollider in actorColliders)
        {
            if (actorCollider.gameObject.activeSelf)
            {
                actorCollider.enabled = value;
            }
        }
        var childColliders = this.user.gameObject.GetComponentsInChildren<Collider2D>();
        foreach(var actorCollider in childColliders)
        {
            if (actorCollider.gameObject.activeSelf)
            {
                actorCollider.enabled = value;
            }
        }
    }

    protected override int InternInvoke(params Actor[] args)
    {
        target = args[0];
        user.myMovement.MoveActor(Vector2.up);
        user.myAnimationHandler.animateWalk();
        return 0;
    }

    IEnumerator CheckForFinish()
    {
        user.myAnimationHandler.Animator.SetTrigger("wall_crawl");
        yield return new WaitForSeconds(2f);
        user.myMovement.MoveActor(Vector2.zero);
        indulgenceCrush.overrideCooldown = true;
        indulgenceCrush.useTrackingCrush = true;
        indulgenceCrush.SetTotalAbilityDuration(0.25f, 1.5f, 0.5f, 0.5f, 1f);
        indulgenceCrush.Invoke(ref this.user, target);
        yield return new WaitForSeconds(1f);
        while (!indulgenceCrush.getIsFinished())
        {
            yield return new WaitForFixedUpdate();
        }
        SetupColliders(true);
        indulgenceCrush.overrideCooldown = false;
        indulgenceCrush.useTrackingCrush = false;
        isFinished = true;
        wallReached = false;
    }

    void OnCollisionStay2D(Collision2D collider)
    {
        if (!isFinished && collider.gameObject.tag == "Environment" && !wallReached)
        {
            wallReached = true;
            SetupColliders(false);
            StartCoroutine(CheckForFinish());
        }
    }
}
