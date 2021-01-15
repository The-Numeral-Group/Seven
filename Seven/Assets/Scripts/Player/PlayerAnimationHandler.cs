using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : ActorAnimationHandler
{
    public Animator playerAnimator;
    public Vector2 movementDirection;
    public bool isMoving;

    void Start()
    {
        playerAnimator = this.gameObject.GetComponent<Animator>();
        this.movementDirection = Vector2.zero;
    }

    public override void animateWalk(Vector2 movementDirection)
    {
        this.movementDirection = movementDirection;
        checkWalking();
        if (this.isMoving)
        {
            playerAnimator.SetBool("player_walking", true);
            playerAnimator.SetFloat("player_H", movementDirection.x);
            playerAnimator.SetFloat("player_V", movementDirection.y);
        }
        else
        {
            playerAnimator.SetBool("player_walking", false);
            playerAnimator.SetFloat("player_H", 0);
            playerAnimator.SetFloat("player_V", 0);
        }

    }

    public void checkWalking()
    {
        if (this.movementDirection == Vector2.zero)
        {
            this.isMoving = false;
        }
        else
        {
            this.isMoving = true;
        }
    }
}
