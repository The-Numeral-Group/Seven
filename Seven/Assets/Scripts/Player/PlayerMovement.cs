using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//we get the character controller from ActorMovement
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : ActorMovement
{
    // Start is called before the first frame update
    /*void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    ///DEBUG
    void OnInteract()
    {
        Debug.Log("PlayerMovement: interact input");
    }
    ///DEBUG

    void OnMovement(InputValue input)
    {
        //Debug.Log(input);
        base.MoveActor(input.Get<Vector2>());
    }

    public override void AnimateWalkActor()
    {
        hostActor.myAnimationHandler.animateWalk();
    }
}
