using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class IndulgenceCharge : ActorAbilityFunction<Actor, int>
{
    [Tooltip("Multiplier value for how fast the charge is. Uses an actors base speed as mutliplicand.")]
    public float chargeSpeedMultiplier = 1.5f;
    [Tooltip("The ammount of charges that will be performed.")]
    public int chargeCount = 3;
    [Tooltip("How long the actor will track the target for.")]
    public float trackTime = 1f;
    [Tooltip("The delay before the charge launches.")]
    public float chargeDelay = 1f;
    [SerializeField]
    [Tooltip("The direction telegraph image for the charge.")]
    protected GameObject directionIndicatorPrefab;
    //Reference to actual instantiated directionindicator
    GameObject directionIndicator;
    //Default facing direction of the indicator
    Vector2 defaultFacingDirection;
    //Coroutine references
    IEnumerator ChargeRoutine;
    IEnumerator TrackRoutine;
    IEnumerator MovementLockroutine;
    IEnumerator TotalRoutine;
    //Stores the charge direction after tracking the target
    Vector2 chargeDirection;
    //Flags which indicate the state the ability is in.
    bool isTracking;
    bool isCharging;
    bool hasCollided;
    //target being charged at.
    Actor target;

    void Awake()
    {
        target = null;
        chargeDirection = Vector2.right;
        defaultFacingDirection = Vector2.right;
        if (directionIndicatorPrefab == null)
        {
            Debug.LogWarning("IndulgenceCharge: directionIndicator prefab not attached.");
            directionIndicator = new GameObject();
            directionIndicator.transform.parent = this.gameObject.transform;
        }
        else
        {
            directionIndicator = Instantiate(directionIndicatorPrefab, this.gameObject.transform);
        }
        directionIndicator.SetActive(false);
    }

    void Start()
    {
        
    }
    public override void Invoke(ref Actor user)
    {
        Debug.LogWarning("IndulgenceCharge: please use the Invoke(ref actor, param object[] args)");
    }

    public override void Invoke(ref Actor user, params object[] args)
    {
        this.user = user;
        //by default, Invoke just does InternInvoke with the provided arguments
        //it's also just implicitly convert the args and give it to InternInvoke
        if(usable && isFinished)
        {
            isFinished = false;
            InternInvoke(easyArgConvert(args));
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }


    protected override int InternInvoke(params Actor[] args)
    {
        MovementLockroutine = this.user.myMovement.LockActorMovement((20f + trackTime) * chargeCount);
        TotalRoutine = ChargeSequence();
        target = args[0];
        this.user.myAnimationHandler.Animator.SetBool("charging", true);
        StartCoroutine(MovementLockroutine);
        StartCoroutine(TotalRoutine);
        return 0;
    }

    IEnumerator TrackTarget()
    {
        isTracking = true;
        directionIndicator.SetActive(true);
        while (isTracking && target != null)
        {
            chargeDirection = (target.transform.position - this.user.gameObject.transform.position).normalized;
            this.user.myAnimationHandler.Flip(chargeDirection);
            float dtheta = Mathf.Acos(((Vector2.Dot(chargeDirection, defaultFacingDirection)) / (chargeDirection.magnitude * defaultFacingDirection.magnitude)));
            if (chargeDirection.y < 0)
            {
                dtheta *= -1;
            }
            dtheta = dtheta * (180/Mathf.PI);
            directionIndicator.transform.localPosition = new Vector3(chargeDirection.x, chargeDirection.y, 0);
            directionIndicator.transform.localRotation = Quaternion.Euler(0, 0, dtheta);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Charge()
    {
        yield return new WaitForSeconds(trackTime);
        isTracking = false;
        yield return new WaitForSeconds(chargeDelay);
        directionIndicator.SetActive(false);
        this.user.myMovement.DragActor(
            chargeDirection * chargeSpeedMultiplier  * this.user.myMovement.speed);
        isCharging = true;
    }

    IEnumerator ChargeSequence()
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

    IEnumerator FailSafe()
    {
        yield return new WaitForSeconds(chargeDelay + trackTime + 10f);
        Debug.Log("IndulgenceCharge: FailSafe routine started, cancelling ability use.");
        FinishAbilitySequence();
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        if (!isFinished && isCharging)
        {
            if (collider.gameObject.tag == "Environment")
            {
                isCharging = false;
                Camera.main.GetComponent<BaseCamera>().Shake(2.0f, 0.2f);
                this.user.myMovement.DragActor(Vector2.zero);
                hasCollided = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isFinished && isCharging)
        {
            if (collider.gameObject.tag == "Player")
            {
                var enemyHealth = collider.gameObject.GetComponent<ActorHealth>();

                //or a weakpoint if there's no regular health
                if(enemyHealth == null){collider.gameObject.GetComponent<ActorWeakPoint>();}

                //if the enemy can take damage (if it has an ActorHealth component),
                //hurt them. Do nothing if they can't take damage.
                if(enemyHealth != null){
                    if (!enemyHealth.vulnerable)
                    {
                        return;
                    }
                    enemyHealth.takeDamage(1);
                }
            }
        }
    }

    void FinishAbilitySequence()
    {
        StopCoroutine(MovementLockroutine);
        StopCoroutine(TotalRoutine);
        StopCoroutine(TrackRoutine);
        StopCoroutine(ChargeRoutine);
        StartCoroutine(this.user.myMovement.LockActorMovement(-1f));
        directionIndicator.transform.localPosition = defaultFacingDirection;
        directionIndicator.transform.localRotation = Quaternion.identity;
        directionIndicator.SetActive(false);
        this.user.myAnimationHandler.Animator.SetBool("charging", false);
        this.user.myMovement.DragActor(Vector2.zero);
        chargeDirection = Vector2.right;
        hasCollided = false;
        isTracking = false;
        isCharging = false;
        isFinished = true;
    }
}
