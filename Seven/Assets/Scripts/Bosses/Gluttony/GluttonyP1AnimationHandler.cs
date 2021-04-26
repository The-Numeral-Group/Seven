using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Document Link: https://docs.google.com/document/d/1zNXt1uYxP-9yh-M8K9UClxZZr4pVskpnNyRH_SD-eXI/edit?usp=sharing
public class GluttonyP1AnimationHandler : ActorAnimationHandler
{
    //control the facing direction of the objects animator. could bs substituted with faceanchor.
    public virtual void AnimateWalk(bool value, Vector2 direction)
    {
        Flip(direction);
        Animator.SetBool("isWalking", value);
    }

    public void ResetLocalRotation()
    {
        if (transform.localScale.x < 0)
        {
            Vector3 flip = transform.localScale;
            flip.x *= -1;
            transform.localScale = flip;
        }
    }
}
