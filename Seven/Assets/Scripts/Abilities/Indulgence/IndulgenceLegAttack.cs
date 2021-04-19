﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndulgenceLegAttack : ActorAbilityFunction<Vector2, int>
{
    public GameObject indulgenceLegPrefab;
    GameObject indulgenceLeg;
    Animator indulgenceLegAnimator;
    Vector2 defaultFacingDirection = Vector2.right;

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
        StartCoroutine(user.myMovement.LockActorMovement(-1f));
        indulgenceLeg.transform.parent = this.user.transform;
        Vector2 direction = args[0];
        direction = direction.normalized;
        float dtheta = Mathf.Acos(((Vector2.Dot(direction, defaultFacingDirection)) / (direction.magnitude * defaultFacingDirection.magnitude)));
        if (direction.y < 0 && direction.x < 0)
        {
            dtheta += Mathf.PI/2;
        }
        else if (direction.y < 0 && direction.x >= 0)
        {
            dtheta += 3*Mathf.PI/2;
        }
        dtheta = dtheta * (180/Mathf.PI);
        indulgenceLeg.transform.localPosition = new Vector3(direction.x, direction.y, 0);
        indulgenceLeg.transform.localRotation = Quaternion.Euler(0, 0, dtheta);
        indulgenceLeg.SetActive(true);
        StartCoroutine(CheckIfAnimationFinished("LegExtend"));
        return 0;
    }

    IEnumerator CheckIfAnimationFinished(string animationName)
    {
         while(indulgenceLegAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            yield return new WaitForFixedUpdate();
        }
        indulgenceLeg.transform.localPosition = Vector3.right;
        indulgenceLeg.transform.localRotation = Quaternion.identity;
        indulgenceLeg.SetActive(false);
        StartCoroutine(user.myMovement.LockActorMovement(0.0001f));
        isFinished = true;
    }
}
