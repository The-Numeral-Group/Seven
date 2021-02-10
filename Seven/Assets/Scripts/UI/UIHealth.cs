using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Temporary UI(Health) for Testing purpose.
public class UIHealth : MonoBehaviour
{
    public GameObject actor;
    public Text health;

    private ActorHealth actorHealth; 
    private float currHealth;

    // Start is called before the first frame update
    void Start()
    {
        this.actorHealth = actor.GetComponent<ActorHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 actorPos = actor.transform.position;
        Vector2 newPos = Vector2.zero;

        // I (Mingun) know this is just hardcoding coordinates based on the actor type.
        // Just keep in mind that this UI script is just temporary and will definitely be changed in the future.

        if (actor.gameObject.tag == "Player")
        {
            if (actorHealth.currentHealth >= 10)
            {
                newPos.x = actorPos.x + 0.5f;
            }
            else
            {
                newPos.x = actorPos.x + 0.8f;
            }
            newPos.y = actorPos.y + 1.2f;
        }
        else // Ghost Knight
        {
            if (actorHealth.currentHealth >= 10)
            {
                newPos.x = actorPos.x + 0.5f;
            }
            else
            {
                newPos.x = actorPos.x + 0.8f;
            }
            newPos.y = actorPos.y + 3.5f;
        }

        transform.position = newPos;
        health.text = "HP: " + actorHealth.currentHealth.ToString();
    }
}
