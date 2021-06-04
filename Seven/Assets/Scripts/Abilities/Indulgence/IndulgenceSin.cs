using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(IndulgenceCrush))]
public class IndulgenceSin : ActorAbilityFunction<Actor, int>
{
    public IndulgenceCrush crushAbility;
    public float maxCorrutionVfxDistance = 5f;
    public Volume corruptionVfx;
    public Actor dummyLocation;
    public GameObject indulgenceSinObject;
    public float rangeToDisengageAttack = 5f;
    List<Vector3> spawnPoints;
    Actor target;
    bool targetInRange;
    IEnumerator CheckRangeRoutine;
    IEnumerator MovementLockRoutine;
    List<IndulgenceSinInteractable> monitors;
    float lerpT;

    void Awake()
    {
        lerpT = 0;
        monitors = new List<IndulgenceSinInteractable>();
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
        spawnPoints.Add(dummyLocation.transform.position + new Vector3(5, -1 * rangeToDisengageAttack, 0));
        spawnPoints.Add(dummyLocation.transform.position + new Vector3(0, -1 * rangeToDisengageAttack, 0));
        spawnPoints.Add(dummyLocation.transform.position + new Vector3(-5, -1 * rangeToDisengageAttack, 0));
        spawnPoints.Add(dummyLocation.transform.position + new Vector3(-10, -1 * rangeToDisengageAttack, 0));
        spawnPoints.Add(dummyLocation.transform.position + new Vector3(10, -1 * rangeToDisengageAttack, 0));
        spawnPoints.Add(dummyLocation.transform.position + new Vector3(-15, -1 * rangeToDisengageAttack, 0));
        spawnPoints.Add(dummyLocation.transform.position + new Vector3(15, -1 * rangeToDisengageAttack, 0));
        monitors = new List<IndulgenceSinInteractable>();
        IndulgenceSinInteractable.TOTAL_CONSUMED = 0;
    }

    void FixedUpdate()
    {
        CalculatePostProcessingWeight();
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
        while(crushAbility.getIsFinished() != true)
        {
            target.myMovement.DragActor(Vector2.down * 0.2f);
            yield return new WaitForFixedUpdate();
        }
        StartCoroutine(MovementLockRoutine);
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            var monitor = Instantiate(indulgenceSinObject, spawnPoints[i], Quaternion.identity);
            monitors.Add(monitor.GetComponent<IndulgenceSinInteractable>());
        }
        yield return new WaitForSeconds(1f);
        StartCoroutine(CheckRangeRoutine);
    }

    IEnumerator CalculateDistanceToTarget()
    {
        while (!targetInRange && target)
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
        for(int i = 0; i < monitors.Count; i++)
        {
            if (monitors[i] && !monitors[i].pickupMode)
            {
                Destroy(monitors[i].gameObject);
                monitors.RemoveAt(i);
                i--;
            }
        }
        CheckRangeRoutine = CalculateDistanceToTarget();
        targetInRange = false;
        this.isFinished = true;
    }

    void CalculatePostProcessingWeight()
    {
        if (IndulgenceSinInteractable.TOTAL_CONSUMED > 0 && target)
        {
            float minMonitorDistance = Mathf.Infinity;
            for(int i = 0; i < monitors.Count; i++)
            {
                if (monitors[i])
                {
                    if (monitors[i].pickupMode)
                    {
                        Vector2 pos = monitors[i].transform.position;
                        float tempDistance = Vector2.Distance(target.transform.position, pos);
                        if (tempDistance < minMonitorDistance)
                        {
                            minMonitorDistance = tempDistance;
                        }
                    }
                }
                else
                {
                    monitors.RemoveAt(i);
                    i--;
                }
            }
            float distanceToWeight = Mathf.Min(minMonitorDistance, maxCorrutionVfxDistance);
            float proportionalweight = 1 - (distanceToWeight /maxCorrutionVfxDistance);
            //https://docs.unity3d.com/ScriptReference/Mathf.Lerp.html
            float temp = corruptionVfx.weight;
            corruptionVfx.weight = Mathf.Lerp(temp, proportionalweight, lerpT);//proportionalweight;
            lerpT += 0.75f * Time.deltaTime;
            if (lerpT > 1.0f)
            {
                lerpT = 0f;
            }
        }
    }
}
