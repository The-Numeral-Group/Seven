using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAnimationHandler : MonoBehaviour
{
    protected Actor hostActor;

    public Animator Animator;

    public ActorMovement myMovement;


    private void Start()
    {
        hostActor = this.GetComponent<Actor>();
        Animator = this.gameObject.GetComponent<Animator>();
        this.myMovement = hostActor.myMovement;
    }
    public virtual void animateWalk()
    {

    }

    public virtual void animateAttack()
    {

    }

    public virtual void animateDodge()
    {

    }

}
