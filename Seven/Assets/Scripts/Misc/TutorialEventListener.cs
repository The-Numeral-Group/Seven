using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEventListener : MonoBehaviour
{
    public GameObject gate;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only collide with player
        if (collision.gameObject.tag == "Player")
        {
            // close gate
            gate.SetActive(true);

            // one time use
            this.gameObject.SetActive(false);
        }
    }
}
