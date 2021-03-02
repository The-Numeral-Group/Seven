using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Temporary UI(Health) for Testing purpose.
public class UIHealth : MonoBehaviour
{
    public GameObject actor;
    public Text text;

    private MultiActor multiActor;
    private ActorHealth actorHealth;

    // Start is called before the first frame update
    void Start()
    {
        // Way to check the type of gameObject.
        // gameObject must have either tag "Player" or "Boss" to indicate which type.
        if (actor.tag == "Player")
        {
            // If actor is player, simply just look for the ActorHealth script.
            actorHealth = actor.GetComponent<ActorHealth>();
        }
        else if(actor.tag == "Boss")
        {
            // If actor is boss, get multiActor.
            multiActor = actor.GetComponent<MultiActor>();          
        }
        else
        {
            Debug.Log("UIHealth: GameObject tag is missing!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(actor == null)
        {
            Debug.Log("UIHealth: Actor is null!");
        }
        else
        {
            // If boss, update actorHealth based on which actor it is currently on.
            if(actor.tag == "Boss")
            {
                var currentActor = multiActor.transform.parent;
                actorHealth = currentActor.GetComponent<ActorHealth>();
            }
            text.text = actor.tag + " HP: " + actorHealth.currentHealth.ToString();
        }

    }
}
