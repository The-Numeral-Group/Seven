using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//we get the character controller from ActorMovement
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovment : ActorMovement
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
        this.MoveActor(input.Get<Vector2>());
    }
}
