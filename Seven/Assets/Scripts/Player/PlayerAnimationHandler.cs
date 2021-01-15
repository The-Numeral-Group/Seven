using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : ActorAnimationHandler
{
    public Vector2 movementDirection;

    public override void animateWalk()
    {
        movementDirection = myMovement.movementDirection;
        if (myMovement.isMoving())
        {
            Animator.SetBool("player_walking", true);
            Animator.SetFloat("player_H", movementDirection.x);
            Animator.SetFloat("player_V", movementDirection.y);
        }
        else
        {
            Animator.SetBool("player_walking", false);
            Animator.SetFloat("player_H", 0);
            Animator.SetFloat("player_V", 0);
        }

    }

    /*Since we have no animation right now, I (Mingun) have just added 1 second delay that
     will turn off the animation. When we get to use animation later, I will make sure 
     the animation will be turned off by itself, not with having a delay that turns off the 
     animation.*/
    public override void animateAttack()
    {
        StartCoroutine(tempAnimateAttack());
    }

    private IEnumerator tempAnimateAttack()
    {
        Animator.SetBool("player_attacking", true);
        yield return new WaitForSeconds(1);
        Animator.SetBool("player_attacking", false);
    }
    public override void animateDodge()
    {
        StartCoroutine(tempAnimateDodge());
    }

    private IEnumerator tempAnimateDodge()
    {
        Animator.SetBool("player_dodging", true);
        yield return new WaitForSeconds(1);
        Animator.SetBool("player_dodging", false);
    }

}
