using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : ActorAnimationHandler
{
    public Vector2 movementDirection;

    public ActorMovement myMovement;

    public override void animateWalk()
    {
        myMovement = hostActor.myMovement;
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
        }

    }

    /*Since we have no animation right now, I (Mingun) have just added 1 second delay that
     will turn off the animation. When we get to use animation later, I will make sure 
     the animation will be turned off by itself, not with having a delay that turns off the 
     animation.*/
    public void animateAttack()
    {
        doAnimateAttack();
    }

    private void doAnimateAttack()
    {
        Animator.SetTrigger("player_attacking");
    }
    public void animateDodge()
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
