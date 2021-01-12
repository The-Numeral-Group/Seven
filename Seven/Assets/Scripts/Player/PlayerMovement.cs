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

    void OnMovement(InputValue input)
    {
        //Debug.Log(input.isPressed);
        base.MoveActor(input.Get<Vector2>());
    }
}
