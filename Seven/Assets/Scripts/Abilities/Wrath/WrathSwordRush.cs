using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathSwordRush : ActorAbilityFunction<Actor, int>
{
    public float chargeSpeedMultiplier;
    public int chargeCount;
    public float trackTime;
    public float chargeDelay;

    public GameObject hitbox;

    private IEnumerator ChargeRoutine;
    private IEnumerator TrackRoutine;
    private IEnumerator MovementLockroutine;
    private IEnumerator TotalRoutine;

    private Vector2 chargeDirection;

    private bool isTracking;
    private bool isCharging;
    private bool hasCollided;

    private Actor wrath;
    private Actor target;

    void Awake()
    {
        target = null;
        chargeDirection = Vector2.right;
    }

    public override void Invoke(ref Actor user)
    {
        Debug.LogWarning("WrathSwordRush: please use the Invoke(ref actor, param object[] args)");
    }

    public override void Invoke(ref Actor user, params object[] args)
    {
        if (usable && isFinished)
        {
            isFinished = false;

            wrath = user;
            wrath.myMovement.MoveActor(Vector2.zero);
            wrath.myMovement.DragActor(Vector2.zero);

            InternInvoke(easyArgConvert(args));
        }
    }
    protected override int InternInvoke(params Actor[] args)
    {
        MovementLockroutine = wrath.myMovement.LockActorMovement((20f + trackTime) * chargeCount);
        TotalRoutine = ChargeSequence();
        target = args[0];
        //wrath.myAnimationHandler.Animator.SetBool("charging", true);
        StartCoroutine(MovementLockroutine);
        StartCoroutine(TotalRoutine);
        return 0;
    }

    private IEnumerator TrackTarget()
    {
        isTracking = true;
        while (isTracking && target != null)
        {
            chargeDirection = target.transform.position - wrath.gameObject.transform.position;
            //wrath.myAnimationHandler.Flip(chargeDirection);
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator Charge()
    {
        hitbox.SetActive(false);
        yield return new WaitForSeconds(trackTime);
        isTracking = false;
        WrathAnimationHandler wrathAnimationHandler = wrath.myAnimationHandler as WrathAnimationHandler;
        wrathAnimationHandler.animateSwordRush();
        yield return new WaitForSeconds(chargeDelay);
        hitbox.SetActive(true);
        wrath.myMovement.DragActor(
            chargeDirection * chargeSpeedMultiplier * wrath.myMovement.speed);
        isCharging = true;
    }

    private IEnumerator ChargeSequence()
    {
        IEnumerator FailSafeRoutine = FailSafe();
        for (int i = 0; i < chargeCount; i++)
        {
            StopCoroutine(FailSafeRoutine);
            hasCollided = false;
            ChargeRoutine = Charge();
            TrackRoutine = TrackTarget();
            FailSafeRoutine = FailSafe();
            StartCoroutine(TrackRoutine);
            StartCoroutine(ChargeRoutine);
            StartCoroutine(FailSafeRoutine);
            yield return new WaitUntil(() => hasCollided);
        }
        StopCoroutine(FailSafeRoutine);
        FinishAbilitySequence();

    }

    private IEnumerator FailSafe()
    {
        yield return new WaitForSeconds(chargeDelay + trackTime + 10f);
        Debug.Log("WrathSwordRush: FailSafe routine started, cancelling ability use.");
        FinishAbilitySequence();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isFinished && isCharging)
        {
            if (collision.gameObject.tag == "Player")
            {
                Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>());
            }

            if (collision.gameObject.tag == "Environment")
            {
                isCharging = false;
                Camera.main.GetComponent<BaseCamera>().Shake(2.0f, 0.2f);
                wrath.myMovement.DragActor(Vector2.zero);
                hasCollided = true;
            }
        }
    }

    private void FinishAbilitySequence()
    {
        StopCoroutine(MovementLockroutine);
        StopCoroutine(TotalRoutine);
        StopCoroutine(TrackRoutine);
        StopCoroutine(ChargeRoutine);
        StartCoroutine(wrath.myMovement.LockActorMovement(-1f));
        //wrath.myAnimationHandler.Animator.SetBool("charging", false);
        wrath.myMovement.DragActor(Vector2.zero);
        chargeDirection = Vector2.right;
        hitbox.SetActive(false);
        hasCollided = false;
        isTracking = false;
        isCharging = false;
        isFinished = true;
    }

}
