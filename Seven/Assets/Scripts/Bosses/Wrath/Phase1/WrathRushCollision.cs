using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathRushCollision : MonoBehaviour
{
    public int damage;

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
                playerHealth.takeDamage(this.damage);
            }
        }
    }
}
