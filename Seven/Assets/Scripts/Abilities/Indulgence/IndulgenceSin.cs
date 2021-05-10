using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IndulgenceCrush))]
public class IndulgenceSin : ActorAbilityFunction<Actor, int>
{
    public IndulgenceCrush crushAbility;
    public Actor dummyLocation;
    public GameObject IndulgenceSinObject;
    public float rangeToDisengageAttack = 5f;
    List<Vector3> spawnPoints;
    Actor target;
    bool targetInRange;
    IEnumerator CheckRangeRoutine;
    IEnumerator MovementLockRoutine;

    void Awake()
    {
        if (dummyLocation == null)
        {
            Debug.LogWarning("IndulgenceSin: Dummylocation actor needs to be set.");
            this.enabled = false;
        }
        if (crushAbility == null)
        {
            crushAbility = this.gameObject.GetComponent<IndulgenceCrush>();
        }
        targetInRange = false;
        CheckRangeRoutine = CalculateDistanceToTarget();
        spawnPoints = new List<Vector3>();
        spawnPoints.Add(dummyLocation.transform.position + new Vector3(5, -10, 0));
        spawnPoints.Add(dummyLocation.transform.position + new Vector3(0, -10, 0));
        spawnPoints.Add(dummyLocation.transform.position + new Vector3(-5, -10, 0));
        IndulgenceSinInteractable.TOTAL_CONSUMED = 0;
    }

    void Start()
    {
        
    }

    public override void Invoke(ref Actor user)
    {
        Debug.LogWarning("IndulgenceSin: Do not use ths version of Invoke. Use Invoke(ref actor, para object[])");
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
        target = args[0];
        this.user.myMovement.MoveActor(Vector2.zero);
        MovementLockRoutine = this.user.myMovement.LockActorMovement(100f);
        crushAbility.useTrackingCrush = true;
        crushAbility.overrideCooldown = true;
        crushAbility.SetTotalAbilityDuration(1f, 1f, 0.25f, 0.125f, 0.5f);
        crushAbility.Invoke(ref this.user, dummyLocation);
        StartCoroutine(InternalCoroutine());
        return 0;
    }

    IEnumerator InternalCoroutine()
    {
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(()=> crushAbility.getIsFinished() == true);
        StartCoroutine(MovementLockRoutine);
        StartCoroutine(CheckRangeRoutine);
    }

    IEnumerator CalculateDistanceToTarget()
    {
        while (!targetInRange)
        {
            yield return new WaitForFixedUpdate();
            float distance = Vector2.Distance(this.user.transform.position, target.transform.position);
            if (distance <= rangeToDisengageAttack)
            {
                targetInRange = true;
            }
        }
        FinishAbility();
    }

    void FinishAbility()
    {
        StopCoroutine(MovementLockRoutine);
        StopCoroutine(CheckRangeRoutine);
        StartCoroutine(this.user.myMovement.LockActorMovement(-1f));
        CheckRangeRoutine = CalculateDistanceToTarget();
        targetInRange = false;
        this.isFinished = true;
    }
}
