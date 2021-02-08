using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrideAssistantWeakPoint : ActorWeakPoint
{
    //METHODS--------------------------------------------------------------------------------------
    /*Start is called on the first frame of the scene. For this particular component, we use 
    it to hard code damage never being able to directly harm Pride.*/
    void Start()
    {
        this.bypassDamageResistance = false;
    }

    public override void takeDamage(float damageTaken)
    {
        Debug.Log("PrideAssistantWeakPoint: actually hit");
        base.takeDamage(damageTaken);
    }

    /*What happens when an ActorHealth on this GameObject hits 0 HP. It's used here to actually
    hurt Pride, as Pride takes damage whenever one of its statues (weakpoints) is destroyed.*/
    public void DoActorDeath()
    {
        ///DEBUG
        Debug.Log("PrideAssistantWeakPoint: dead");
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
        ///DEBUG
        
        int totalWeakPoints 
            = this.ownerHealth.gameObject.GetComponent<PrideActor>().weakSpots.Count;

        var dam = (this.ownerHealth.maxHealth / totalWeakPoints);
        dam /= (1.0f - this.ownerHealth.damageResistance);

        this.ownerHealth.takeDamage(dam);

        this.gameObject.GetComponent<PrideSin>().enabled = true;
        this.enabled = false;
    }


}
