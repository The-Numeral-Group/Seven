using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//total consumed gets reset by the onawake function of the indulgencesinability
public class IndulgenceSinInteractable : Interactable
{
    public static int TOTAL_CONSUMED = 0;
    public int healAmount = 1;
    public bool pickupMode { get; set;}
    Actor player;

    void Start()
    {
        var pObject = GameObject.FindGameObjectWithTag("Player");
        if (pObject)
        {
            player = pObject.GetComponent<Actor>();
        }
    }
    public override void OnInteract()
    {
        //possible integet overflow
        TOTAL_CONSUMED += 1;
        player.myHealth.currentHealth = 
            player.myHealth.currentHealth + healAmount > player.myHealth.maxHealth ? 
                player.myHealth.maxHealth : player.myHealth.currentHealth + healAmount;
        if (TOTAL_CONSUMED > 1)
        {
            IndulgenceP1Actor.SIN_COMITTED = true;
        }
        ShowIndicator(false);
        SetPotentialInteractable(false, player.gameObject);
        Destroy(this.gameObject);
    }

    //What happens when the player gets in range of this object
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && pickupMode)
        {
            ShowIndicator(true);
            SetPotentialInteractable(true, other.gameObject);
        }
    }

    //What happens when the player exits the range of this object
    protected override void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" && pickupMode)
        {
            ShowIndicator(false);
            SetPotentialInteractable(false, other.gameObject);
        }
    }

    protected override void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player" && pickupMode)
        {
            if (POTENTIAL_INTERACTABLE && POTENTIAL_INTERACTABLE != this)
            {
                float myDistanceToPlayer = Vector2.Distance(other.transform.position, this.transform.position);
                float closestPotentialDistanceToPlayer = Vector2.Distance(other.transform.position, 
                                                            POTENTIAL_INTERACTABLE.transform.position);
                if (myDistanceToPlayer < closestPotentialDistanceToPlayer)
                {
                    ShowIndicator(true);
                    SetPotentialInteractable(true, other.gameObject);
                }
                else
                {
                    ShowIndicator(false);
                }
            }
            else if (!POTENTIAL_INTERACTABLE)
            {
                ShowIndicator(true);
                SetPotentialInteractable(true, other.gameObject);
            }
        }
    }
}
