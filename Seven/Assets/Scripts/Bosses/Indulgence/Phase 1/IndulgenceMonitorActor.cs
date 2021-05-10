using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MonitorLaser))]
[RequireComponent(typeof(Collider2D))]
public class IndulgenceMonitorActor : Actor
{
    public MonitorLaser mLaser;
    public IndulgenceSinInteractable interactableSin;
    public State currState;
    Actor target;
    public Collider2D myCollider;
    public enum State
    {
        IDLE,
        LASER,
        DEAD,
    }

    protected override void Start()
    {
        currState = State.IDLE;
        base.Start();
        if (SetupTarget())
        {
            currState = State.LASER;
            ExecuteState(currState);
        }
    }
    public override void DoActorDeath()
    {
        mLaser.FinishMonitorLaser();
        this.myHealth.SetVulnerable(false, -1f);
        this.myAnimationHandler.Animator.SetTrigger("death");
        currState = State.DEAD;
        ExecuteState(State.DEAD);
    }

    public override void DoActorDamageEffect(float damage)
    {
        this.myAnimationHandler.Animator.SetTrigger("hit");
        base.DoActorDamageEffect(damage);
    }

    public bool SetupTarget(Actor t = null)
    {
        if (t)
        {
            target = t;
            return true;
        }
        else
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p)
            {
                target = p.GetComponent<Actor>();
                return true;
            }
            return false;
        }
    }

    void ExecuteState(State state)
    {
        switch(state)
        {
            case State.LASER:
                Actor self = this;
                mLaser.Invoke(ref self, target);
                break;
            case State.IDLE:
                break;
            case State.DEAD:
                myCollider.isTrigger = true;
                interactableSin.pickupMode = true;
                break;
            default:
                break;
        }
    }
}
