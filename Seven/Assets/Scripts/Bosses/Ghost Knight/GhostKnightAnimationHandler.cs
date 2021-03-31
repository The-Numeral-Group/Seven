using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Doc: https://docs.google.com/document/d/1IsFxZFPXsj36T4mAHAlGyEoYOOlxWjoatP55CUwg2ms/edit
public class GhostKnightAnimationHandler : ActorAnimationHandler
{
    /*public override void Start()
    {
        hostActor = this.GetComponent<Actor>();

        GameObject child = transform.GetChild(1).gameObject;

        Animator = child.GetComponent<Animator>();
    }*/


    public void animateVSlash()
    {
        Animator.SetTrigger("vSlash");
        //Debug.Log(anim["vSlash"].time);
    }

    public void animateHSlash()
    {
        Animator.SetTrigger("hSlash");
        //Debug.Log(anim["hSlash"].time);
    }
}
