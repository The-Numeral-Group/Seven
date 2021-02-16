using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEventListener : MonoBehaviour
{
    public GameObject gate;

    void OnCollisionEnter2D(Collision2D collider)
    {
        // Only collide with player
        if (collider.gameObject.tag == "Player")
        {
            // close gate
            gate.SetActive(true);
            
            // one time use
            this.gameObject.SetActive(false);
        }
    }
}
