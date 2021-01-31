using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostKnightAnimationHandler : ActorAnimationHandler
{
    public Vector2 movementDirection;

    public ActorMovement myMovement;

    public override void animateWalk()
    {
        myMovement = hostActor.myMovement;
        movementDirection = myMovement.movementDirection;
    }
}
