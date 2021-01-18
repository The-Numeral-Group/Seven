using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*I (Thomas) am genericing the return type to int because 
I don't care about the return*/
public class Dodge : ActorAbilityFunction<ActorMovement, int>
{
    //how far the actor goes when they doddge
    public float dodgeDistance = 15.0f;

    //how long before the actor can move after dodging
    public float movementLockForDodge = 1.0f;

    /*// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    public override void Invoke(ref Actor user)
    {
        if(usable)
        {
            isFinished = false;
            StartCoroutine(coolDown(cooldownPeriod));
            InternInvoke(user.myMovement);
        }
    }

    protected override int InternInvoke(params ActorMovement[] args)
    {
        //we assume that the needed ActorMovement is the first thing
        //in args, that's what the 0 is for.

        Vector2 velocity = args[0].movementDirection;
        float drag = args[0].rigidbody.drag;

        /*to dodge, we boost forward and lock movement for 1 second. This calculation
        was written by Ram for the prototype*/
        Vector2 dodgeVelocity = velocity + Vector2.Scale(velocity, dodgeDistance * 
            new Vector2((Mathf.Log(1f/ (Time.deltaTime * drag + 1))/-Time.deltaTime),
                (Mathf.Log(1f/ (Time.deltaTime * drag + 1))/-Time.deltaTime)));
        
        args[0].DragActor(dodgeVelocity);
        StartCoroutine(args[0].LockActorMovement(movementLockForDodge));
        isFinished = true;
        return 0;
    }
}
