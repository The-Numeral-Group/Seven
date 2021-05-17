using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathChainCollision : MonoBehaviour
{
    private Actor player;
    private GameObject wrath;

    private void Awake()
    {
        var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];
        player = playerObject.GetComponent<Actor>();
        wrath = GameObject.FindGameObjectsWithTag("Boss")?[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            var playerHealth = collision.gameObject.GetComponent<ActorHealth>();

            if (playerHealth == null) { collision.gameObject.GetComponent<ActorWeakPoint>(); }

            if (playerHealth != null)
            {
                if (!playerHealth.vulnerable)
                {
                    return;
                }
                this.wrath.SendMessage("onChainCollision");
            }
        }
    }
}
