using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*So, it turns out that many character controller modules 
have a ton of fluff we don't need. This is a super
lightweight controller to keep everything simple.
If something needs more complicated movment, we can*/

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleController2D : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private Vector2 movementDirection;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        movementDirection = Vector2.zero;
    }

    // FixedUpdate is called once every 1/60 of a second
    void FixedUpdate()
    {
        rb2D.MovePosition(rb2D.position + movementDirection);
    }

    //updates movement direction so things move good
    public void Move(Vector2 movemenet)
    {
        movementDirection = movemenet; 
    }
}
