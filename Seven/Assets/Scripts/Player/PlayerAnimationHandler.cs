using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : ActorAnimationHandler
{
    public Vector2 movementDirection;

    public ActorMovement myMovement;

    private float prevX, prevY;

    public override void animateWalk()
    {
        myMovement = hostActor.myMovement;
        movementDirection = myMovement.movementDirection;
        if (myMovement.isMoving())
        {
            Animator.SetBool("player_walking", true);
            Animator.SetFloat("player_H", movementDirection.x);
            Animator.SetFloat("player_V", movementDirection.y);
            prevX = movementDirection.x;
            prevY = movementDirection.y;
        }
        else
        {
            Animator.SetBool("player_walking", false);
        }

    }

    public void animateAttack()
    {
        doAnimateAttack();
    }

    private void doAnimateAttack()
    {
        Animator.SetFloat("playerAttack_H", prevX);
        Animator.SetFloat("playerAttack_V", prevY);

        Animator.SetTrigger("player_attacking");
    }
    public void animateDodge()
    {
        doAnimateDodge();
    }

    private void doAnimateDodge()
    {
        Animator.SetBool("player_walking", false);
        Animator.SetFloat("playerDodge_H", movementDirection.x);
        Animator.SetFloat("playerDodge_V", movementDirection.y);
        Animator.SetTrigger("player_dodging");
    }

    public void animateRespawn()
    {
        Animator.SetTrigger("player_respawn");
    }

}
