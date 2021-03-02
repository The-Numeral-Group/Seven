using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Source: https://www.youtube.com/watch?v=BLfNP4Sc_iA&ab_channel=Brackeys
public class UIHealthBar : MonoBehaviour
{
    public GameObject actor;
    public Slider slider;

    private MultiActor multiActor;
    private ActorHealth actorHealth;

    private bool maxHealthInserted = false;

    void Start()
    {
        // Way to check the type of gameObject.
        // gameObject must have either tag "Player" or "Boss" to indicate which type.
        if (actor.tag == "Player")
        {
            // If actor is player, simply just look for the ActorHealth script.
            actorHealth = actor.GetComponent<ActorHealth>();
            SetMaxHealth((int)actorHealth.currentHealth);
            maxHealthInserted = true;
        }
        else if (actor.tag == "Boss")
        {
            // If actor is boss, get multiActor.
            multiActor = actor.GetComponent<MultiActor>();
        }
        else
        {
            Debug.Log("UIHealthBar: GameObject tag is missing!");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (actor == null)
        {
            Debug.Log("UIHealth: Actor is null!");
        }
        else
        {
            // If boss, update actorHealth based on which actor it is currently on.
            if (actor.tag == "Boss")
            {
                var currentActor = multiActor.transform.parent;
                actorHealth = currentActor.GetComponent<ActorHealth>();
            }
            if (!this.maxHealthInserted)
            {
                // Boss' maxHealth cannot be setted in the Start() function 
                // because during that time, Boss MultiActor is still did not choose the first phase of Boss.
                // Therefore, Boss' maxHealth gets set up in here.
                SetMaxHealth((int)actorHealth.currentHealth);
                maxHealthInserted = true;
            }

            SetHealth((int)actorHealth.currentHealth);
        }

    }


    public void SetMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }
    
    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
