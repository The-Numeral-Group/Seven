using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorWeakPoint : ActorHealth
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("The ActorHealth that this weak point is weak for.")]
    public ActorHealth ownerHealth;

    [Tooltip("How much damage done to this weak point should be increased for when it is" + 
        " applied to the owner's health.")]
    public float damageMultiplier = 1.0f;

    [Tooltip("If the owner has any damage resistance, whether or not damage from this" + 
        " weak point should bypass it.")]
    public bool bypassDamageResistance = true;

    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        if(ownerHealth == null)
        {
            var parent = this.gameObject.transform.parent;
            ownerHealth = parent != null ? parent.gameObject.GetComponent<ActorHealth>() : null;

            if(ownerHealth == null)
            {
                Debug.LogWarning("ActorWeakPoint: Weakpoint GameObject starting with no owner," + 
                    " damage cannot be sent anywhere.");
            }
        }
    }

    /*Similar to ActorHealth, the method to call when this component should take damage. However.
    this version has special functionality to interact with damage resistance of related
    ActorHealths.*/

    //new in a method declaration means "use me rather than my superclass's version"
    public override void takeDamage(float damageTaken){
        if (!this.vulnerable)
        {
            return;
        }
        //take the damage to the weakpoint
        this.currentHealth -= Mathf.Floor(damageTaken * (1.0f - damageResistance));

        /*then deal the damage to the owner. When bypassing damage resistance, the damage
        is divided by 1 minus the owner's damage resistance, which mathematically cancels
        it out. If the damage doesn't get to bypass resistance, it's dealt like normal.*/
        if(bypassDamageResistance)
        {
            var dam = (damageTaken * damageMultiplier) / (1.0f - ownerHealth.damageResistance);
            ownerHealth.takeDamage(dam);
        }
        else
        {
            ownerHealth.takeDamage(damageTaken * damageMultiplier);
        }

        //if the attack killed the thing
        if(this.currentHealth <= 0){
            /*I'd like to use SendMessageOptions.RequireReciever to make it so
            that the game vomits if we try to kill something that cannot die,
            but I just don't know how*/
            this.gameObject.SendMessage("DoActorDeath");//, null, RequireReciever);
        }
    }
}
