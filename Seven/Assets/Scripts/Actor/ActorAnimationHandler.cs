using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAnimationHandler : MonoBehaviour
{
    protected Actor hostActor;

    void Start()
    {
        hostActor = this.GetComponent<Actor>();
    }
    public virtual void animateWalk(Vector2 movementDirection)
    {

    }

}
