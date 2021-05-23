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
    public GameObject targetPoint;

    private IEnumerator ChargeRoutine;
    private IEnumerator TrackRoutine;
    private IEnumerator MovementLockroutine;
    private IEnumerator TotalRoutine;

    private Vector2 chargeDirection;
    private Vector3 targetLocation;

    private bool isCharging;
    private bool hasArrived;
    private bool isTracking;

    private Actor wrath;
    private Actor target;

    private GameObject targetPointObject;
    [SerializeField]
    [Tooltip("The direction telegraph image for the charge.")]
    protected GameObject directionIndicatorPrefab;
    //Reference to actual instantiated directionindicator
    GameObject directionIndicator;
    //Default facing direction of the indicator
    Vector2 defaultFacingDirection;

    void Awake()
    {
        target = null;
        chargeDirection = Vector2.right;
        defaultFacingDirection = Vector2.right;
        if (directionIndicatorPrefab == null)
        {
            Debug.LogWarning("WrathSwordRush: directionIndicator prefab not attached.");
            directionIndicator = new GameObject();
            directionIndicator.transform.parent = this.gameObject.transform;
        }
        else
        {
            directionIndicator = Instantiate(directionIndicatorPrefab, this.gameObject.transform);
        }
        directionIndicator.SetActive(false);
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
        directionIndicator.SetActive(true);
        while (isTracking && target != null)
        {
            chargeDirection = (target.transform.position - wrath.gameObject.transform.position).normalized;
            targetLocation = target.transform.position;
            float dtheta = Mathf.Acos(((Vector2.Dot(chargeDirection, defaultFacingDirection)) / (chargeDirection.magnitude * defaultFacingDirection.magnitude)));
            if (chargeDirection.y < 0)
            {
                dtheta *= -1;
            }
            dtheta = dtheta * (180/Mathf.PI);
            directionIndicator.transform.localPosition = new Vector3(chargeDirection.x, chargeDirection.y, 0);
            directionIndicator.transform.localRotation = Quaternion.Euler(0, 0, dtheta);
            if (chargeDirection.x > 0.0f) {
                targetLocation.x += 7.5f;
            }
            else
            {
                targetLocation.x += -7.5f;
            }

            if (chargeDirection.y > 0.0f)
            {
                targetLocation.y += 7.5f;
            }
            else
            {
                targetLocation.y += -7.5f;
            }

            //wrath.myAnimationHandler.Flip(chargeDirection);
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator Charge()
    {
        hitbox.SetActive(false);
        yield return new WaitForSeconds(trackTime);

        isTracking = false;

        // Play Wrath charging animation
        WrathAnimationHandler wrathAnimationHandler = wrath.myAnimationHandler as WrathAnimationHandler;
        wrathAnimationHandler.animateSwordRush();

        // Place target Point
        targetPointObject = Instantiate(this.targetPoint, targetLocation, Quaternion.identity);

        yield return new WaitForSeconds(chargeDelay);
        directionIndicator.SetActive(false);
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
            hasArrived = false;
            ChargeRoutine = Charge();
            FailSafeRoutine = FailSafe();
            TrackRoutine = TrackTarget();
            StartCoroutine(TrackRoutine);
            StartCoroutine(ChargeRoutine);
            StartCoroutine(FailSafeRoutine);
            yield return new WaitUntil(() => hasArrived);
            Destroy(targetPointObject);
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

            if (collision.gameObject.tag == "Target Point" || collision.gameObject.tag == "Environment")
            {
                isCharging = false;
                wrath.myMovement.DragActor(Vector2.zero);
                hasArrived = true;
            }

            /*if (collision.gameObject.tag == "Environment")
            {
                isCharging = false;
                Camera.main.GetComponent<BaseCamera>().Shake(2.0f, 0.2f);
                wrath.myMovement.DragActor(Vector2.zero);
                hasCollided = true;
            }*/
        }
    }

    private void FinishAbilitySequence()
    {
        StopCoroutine(MovementLockroutine);
        StopCoroutine(TotalRoutine);
        StopCoroutine(ChargeRoutine);
        StopCoroutine(TrackRoutine);
        StartCoroutine(wrath.myMovement.LockActorMovement(-1f));
        //wrath.myAnimationHandler.Animator.SetBool("charging", false);
        wrath.myMovement.DragActor(Vector2.zero);
        chargeDirection = Vector2.right;
        directionIndicator.transform.localPosition = defaultFacingDirection;
        directionIndicator.transform.localRotation = Quaternion.identity;
        directionIndicator.SetActive(false);
        hitbox.SetActive(false);
        hasArrived = false;
        isCharging = false;
        isTracking = false;
        isFinished = true;
    }

}
