using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostKnightAnimationHandler : ActorAnimationHandler
{

    public void animateVSlash()
    {
        Animator.SetTrigger("vSlash");
    }

    public void animateHSlash()
    {
        Animator.SetTrigger("hSlash");
    }
}
