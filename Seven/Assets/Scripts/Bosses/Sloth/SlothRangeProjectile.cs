using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlothRangeProjectile : BasicProjectile
{
    //FIELDS---------------------------------------------------------------------------------------
    public enum SlothProjectileNature
    {
        STRAIGHT,
        ARC
    }

    [Tooltip("The nature of how this projectile moves. STRAIGHT will move directly" + 
        " to target, while ARC will arc up (on the positive y axis) then back down to target." + 
            " Keep in mind that ARC movement will take longer and travel a longer distance.")]
    public SlothProjectileNature movementNature = SlothProjectileNature.STRAIGHT;

    //METHODS--------------------------------------------------------------------------------------
    //What happens when the projectile actually hits something
    protected override void OnTriggerEnter2D(Collider2D collided)
    {
        //send the marker activation message to whatever was hit and self-destruct
        //the options argument will supress errors if the collided object has no such method
        collided.gameObject.SendMessage("OnActivateMarker", 
            SendMessageOptions.DontRequireReceiver);

        Destroy(this.gameObject);
    }

    protected override void InternalMovement(Vector2 movementDirection)
    {
        //basic next step: from the direction towards target, figure out how to do projectile
        //arcs
    }
}
