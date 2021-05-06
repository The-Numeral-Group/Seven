using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class ActorAnimationHandler : MonoBehaviour
{
    protected Actor hostActor;

    public Animator Animator;
    public SpriteRenderer sp;

    public virtual void Start()
    {
        hostActor = this.GetComponent<Actor>();
        Animator = this.gameObject.GetComponent<Animator>();
        sp = this.gameObject.GetComponent<SpriteRenderer>();
    }
    
    /*For now, all the actors will have walkAnimation, so only this function is included
      this class. However, all the actors will have different ability sets, so they 
      will be included in their own AnimationHandler class. For example, you will be 
      able to find animateAttack() and animateDodge() in PlayerAnimationHandler but not
      in here.*/
    public virtual void animateWalk()
    {

    }

    //Helper function for flipping a funtions sprite along the y axis.
    public virtual void Flip(Vector2 direction)
    {
        if (direction.x < 0) //left
        {
            sp.flipX = true;
        }
        else if (direction.x > 0) //right
        {
            sp.flipX = false;
        }
    }

    //Checks if this animator has a trigger named triggerName. If so, trigger it
    //and return true. If not, print a suppressable warning to console and return
    //false
    public bool TrySetTrigger(string triggerName, bool suppressWarnings=false)
    {
        //Does the trigger exist?
        bool triggerExists = 
            //Please search the AnimatorControllerParameter array...
            System.Array.Find<AnimatorControllerParameter>(
                Animator.parameters,    //called Animator.parameters...
                //for a value that is Trigger type and has this name
                param => param.type == AnimatorControllerParameterType.Trigger 
                    && param.name == triggerName
            //Find returns the default of the generic type if no match is found
            ) != default(AnimatorControllerParameter);

        //If it does
        if(triggerExists)
        {
            //Trigger it and go
            Animator.SetTrigger(triggerName);
            return true;
        }
        //If it doesn't...
        else if(!suppressWarnings)
        {
            //Throw a warning, if so desired
            Debug.LogWarning($"ActorAnimationHandler: Animator"
                + $" {Animator.GetType().Name} has no trigger named {triggerName}");
        }

        return false;
    }  
}
