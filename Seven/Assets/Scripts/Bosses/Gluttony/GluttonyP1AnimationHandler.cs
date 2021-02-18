using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GluttonyP1AnimationHandler : ActorAnimationHandler
{
    //control the facing direction of the objects animator. could bs substituted with faceanchor.
    bool facingRight = true;
    public virtual void AnimateWalk(bool value, Vector2 direction)
    {
        Flip(direction);
        Animator.SetBool("isWalking", value);
    }

    public void Flip(Vector2 direction)
    {
        Vector3 flip = transform.localScale;
        if (direction.x < 0 && facingRight)
        {
            flip.x *= -1;
            facingRight = false;
        }
        else if (direction.x > 0 && !facingRight)
        {
            flip.x *= -1;
            facingRight = true;
        }
        transform.localScale = flip;
    }
}
