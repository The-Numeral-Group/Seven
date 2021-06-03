using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndulgenceLegAttack : ActorAbilityFunction<Vector2, int>
{
    public GameObject indulgenceLegPrefab;
    GameObject indulgenceLeg;
    Animator indulgenceLegAnimator;
    Vector2 defaultFacingDirection = Vector2.right;
    bool animationTriggered;
    IEnumerator MovementLockRoutine;
    IEnumerator AnimationCheckRoutine;
    void Start()
    {
        if (indulgenceLegPrefab)
        {
            indulgenceLeg = Instantiate(indulgenceLegPrefab, this.transform);
            indulgenceLegAnimator = indulgenceLeg.GetComponent<Animator>();
            indulgenceLeg.transform.localPosition = Vector3.right;
            indulgenceLeg.SetActive(false);
        }
        else
        {
            Debug.LogWarning("IndulgenceLegAttack: IngulgenceLeg prefab not setup properly via inspector.");
            this.enabled = false;
        }
    }
    public override void Invoke(ref Actor user)
    {
        Debug.LogWarning("IndulgenceLegAttack: please use the Invoke(ref actor, param object[] args)");
    }

    public override void Invoke(ref Actor user, params object[] args)
    {
        this.user = user;
        if(usable && this.isFinished)
        {
            isFinished = false;
            InternInvoke(easyArgConvert(args));
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    protected override int InternInvoke(params Vector2[] args)
    {
        AnimationCheckRoutine = CheckIfAnimationFinished("LegExtend");
        this.user.myAnimationHandler.Animator.SetBool("in_physical", true);
        this.user.mySoundManager.PlaySound("leg_attack", 0.8f, 1.2f);
        MovementLockRoutine = user.myMovement.LockActorMovement(20f);
        indulgenceLeg.transform.parent = this.user.transform;
        Vector2 direction = args[0];
        direction = direction.normalized;
        float dtheta = 0;
        if (direction != Vector2.zero)
        {
            dtheta = Mathf.Acos(((Vector2.Dot(direction, defaultFacingDirection)) / (direction.magnitude * defaultFacingDirection.magnitude)));
            this.user.myAnimationHandler.Flip(direction);
        }
        if (direction.y < 0)
        {
            dtheta *= -1;
        }
        dtheta = dtheta * (180/Mathf.PI);
        indulgenceLeg.transform.localPosition = new Vector3(direction.x, direction.y, 0);
        indulgenceLeg.transform.localRotation = Quaternion.Euler(0, 0, dtheta);
        //This coroutine is called in casse the animation does not trigger the start leg attack.
        StartCoroutine(CheckIfAnimationTriggered());
        return 0;
    }

    IEnumerator CheckIfAnimationTriggered()
    {
        yield return new WaitForSeconds(1f);
        if (animationTriggered)
        {
            yield return null;
        }
        else
        {
            StartCoroutine(AnimationCheckRoutine);
        }
    }

    public void StartLegAttack()
    {
        if (!isFinished && !animationTriggered)
        {
            animationTriggered = true;
            StartCoroutine(AnimationCheckRoutine);
        }
    }
    IEnumerator CheckIfAnimationFinished(string animationName)
    {
        //yield return new WaitForSeconds(0.5f);
        indulgenceLeg.SetActive(true);
        while(indulgenceLegAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            yield return new WaitForFixedUpdate();
        }
        indulgenceLeg.SetActive(false);
        this.user.myAnimationHandler.Animator.SetBool("in_physical", false);
        //failsafe if animation triggerss to call finishlegattack
        yield return new WaitForSeconds(10f);
        FinishAttack();
    }

    public void FinishLegAttack()
    {
        FinishAttack();
    }

    void FinishAttack()
    {
        StopCoroutine(AnimationCheckRoutine);
        StopCoroutine(MovementLockRoutine);
        indulgenceLeg.transform.localPosition = Vector3.right;
        indulgenceLeg.transform.localRotation = Quaternion.identity;
        indulgenceLeg.SetActive(false);
        StartCoroutine(user.myMovement.LockActorMovement(-1f));
        animationTriggered = false;
        isFinished = true;
    }
}
