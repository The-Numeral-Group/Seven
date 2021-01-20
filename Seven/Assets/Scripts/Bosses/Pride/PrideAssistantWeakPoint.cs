using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrideAssistantWeakPoint : ActorWeakPoint
{
    //METHODS--------------------------------------------------------------------------------------
    /*Awake is called when a component is set to Active. For this particular component, we use 
    it to hard code damage never being able to directly harm Pride.*/
    void Awake()
    {
        this.bypassDamageResistance = false;
    }

    /*What happens when an ActorHealth on this GameObject hits 0 HP. It's used here to actually
    hurt Pride, as Pride takes damage whenever one of its statues (weakpoints) is destroyed.*/
    public void DoActorDeath()
    {
        var totalWeakPoints = this.ownerHealth.gameObject.GetComponent<PrideActor>().weakSpots.Count;

        var dam = (this.ownerHealth.maxHealth / totalWeakPoints);
        dam /= (1.0f - this.ownerHealth.damageResistance);

        this.ownerHealth.takeDamage(dam);

        this.gameObject.SetActive(false);
    }


}
