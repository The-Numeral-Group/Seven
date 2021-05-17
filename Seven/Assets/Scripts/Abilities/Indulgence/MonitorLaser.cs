using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitorLaser : ActorAbilityFunction<Actor, int>
{
    public GameObject laserObject;
    int layerMask;
    Actor target;
    IEnumerator DetectTargetRoutine;
    IEnumerator FireBeamPTR;

    public override void Invoke(ref Actor user)
    {
        Debug.LogWarning("MonitorLaser: user the other version  of invoke.");
    }
    public override void Invoke(ref Actor user, params object[] args)
    {
        if (isFinished)
        {
            base.Invoke(ref user, args);
        }
    }
    protected override int InternInvoke(params Actor[] args)
    {
        DetectTargetRoutine = DetectPlayer();
        FireBeamPTR = FireBeam();
        target = args[0];
        layerMask = ~(1 << user.gameObject.layer);
        StartCoroutine(DetectTargetRoutine);
        return 0;
    }

    IEnumerator DetectPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(this.user.transform.position, Vector2.down, Mathf.Infinity,layerMask);
        while (hit.collider == null || hit.collider.tag != target.gameObject.tag)
        {
            yield return new WaitForFixedUpdate();
           hit =  Physics2D.Raycast(this.user.transform.position, Vector2.down, Mathf.Infinity,layerMask);
        }
        FireBeamPTR = FireBeam();
        StartCoroutine(FireBeamPTR);
    }
    
    IEnumerator FireBeam()
    {
        this.user.myAnimationHandler.Animator.SetTrigger("projectile_attack");
        yield return new WaitForSeconds(0.5f);
        this.laserObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        this.laserObject.SetActive(false);
        yield return new WaitForSeconds(cooldownPeriod);
        DetectTargetRoutine = DetectPlayer();
        StartCoroutine(DetectTargetRoutine);
    }

    public void FinishMonitorLaser()
    {
        StopCoroutine(DetectTargetRoutine);
        StopCoroutine(FireBeamPTR);
        this.laserObject.SetActive(false);
        isFinished = true;
    }
}
