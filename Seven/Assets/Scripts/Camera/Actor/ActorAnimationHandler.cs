﻿using System.Collections;
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
    
    /*For now, all the actors will have walkAnimation, so only this function is included
      this class. However, all the actors will have different ability sets, so they 
      will be included in their own AnimationHandler class. For example, you will be 
      able to find animateAttack() and animateDodge() in PlayerAnimationHandler but not
      in here.*/
    public virtual void animateWalk()
    {

    }

}