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

    /*[Tooltip("How high the projectile should arc. Increasing this value will make the projectile" + 
        " take longer to arrive at its destination.")]
    public float arcAmplitude = 1f;*/

    [Tooltip("How long an arcing projectile should take to land, relative to how long it" + 
        " takes for direct projectiles to land (as in, if a direct projectile takes X seconds," +
            " an arcing projectile would take X + arcDelay seconds).")]
    public float arcDelay = 2f;

    //the marker that this projectile intends to follow
    public SlothRangeMarker marker{ private get; set; }

    private Vector3 markerPos;

    //the distance to the target
    private float totalDist = 0.0f;

    //the number to multiply position by to get the appropriate sine values for the projectile arc
    //private float sinFactor = 0.0f;

    //METHODS--------------------------------------------------------------------------------------
    //What happens when the projectile actually hits something
    protected override void OnTriggerEnter2D(Collider2D collided)
    {
        //make sure what we're colliding with is not a different range projectile
        if(collided.gameObject.GetComponent<SlothRangeProjectile>() == null)
        {
            ///DEBUG
            Debug.Log("SlothRangeProjectile: collided with " + collided.gameObject.name);
            ///DEBUG
            //deal damage to whatever was hit, if it can take damage
            collided.gameObject.GetComponent<ActorHealth>()?.takeDamage(this.damage);
            //send the marker activation message to whatever was hit and self-destruct
            //the options argument will supress errors if the collided object has no such method
            collided.gameObject.SendMessage("OnActivateMarker", this,
                SendMessageOptions.DontRequireReceiver);

            //destroy the marker, if the collided object isn't the marker
            if(collided.gameObject.GetComponent<SlothRangeMarker>() != marker)
            {
                Destroy(marker.gameObject);
            }

            //self destruct
            Destroy(this.gameObject);
        }
        
    }

    protected override void InternalMovement(Vector2 movementDirection)
    {
        //basic next step: from the direction towards target, figure out how to do projectile
        //arcs: make the mf a sine wave

        //if the projectile wants to move in a straight line
        if(movementNature == SlothProjectileNature.STRAIGHT)
        {
            //just move it forward
            mover.MoveActor(movementDirection);
        }
        //if it isn't
        else
        {
            //sine wave the mf
            /*Specifically, sine wave it in such a way that the period of
            the wave is twice the distance that needs to be travelled,
            so the projectile will arrive at its destination after its
            first arc*/
            var travelledDist = totalDist - Vector2.Distance(
                this.gameObject.transform.position, 
                markerPos
            );

            /*fun fact: if object x and object y are traveling at the same speed,
            but x is moving linearly and y is moving sinusoidally, and x is to arrive
            in time N, and y is to arrive some time Z after x, then 
            y will arrive in time sin((PI / Z) * N)!
            
            Umm... I think. Look the fact that I haven't needed any level of math
            education until this point is the bigger problem.
            
            Also I'm judging off of distance travelled, not time.
            
            Look it's a work in progress.*/
            var rotArc = Mathf.Sin(((float)System.Math.PI / arcDelay) * travelledDist);

            //once calculated, move in the sine'd direction
            mover.MoveActor(rotateHelp(movementDirection, rotArc));
            ///DEBUG
            Debug.DrawLine(this.gameObject.transform.position, this.gameObject.transform.position * 1.01f, Color.red, 30f); 
            ///DEBUG
        }
    }

    /*Starts the projectile! Exactly the same as the original, except it also calculates
    the distance to the target, for easy arc calculation*/
    public new void Launch(Vector2 target, LAUNCH_MODE mode = LAUNCH_MODE.POINT)
    {
        /*if there's a marker, get the distance to it an calculate the needed sine period*/
        if(this.marker != null)
        {
            totalDist = Vector2.Distance(
                this.gameObject.transform.position, 
                this.marker.gameObject.transform.position
            );

            //also save the marker's coordinates
            markerPos = this.marker.gameObject.transform.position;

            //calc sine here, should just be twice the dist since half an oscillation is needed
            //sinFactor = totalDist * 2;
        }
        //if there isn't, that might be a problem.
        else
        {
            Debug.LogWarning("SlothRangeProjectile: Projectile does not have a designated" + 
                " marker and may not be able to arc correctly.");
        }

        base.Launch(target, mode);
    }

    /*Helper for rotating vectors for arc angles. The delta angle is in RADIANS*/
    private Vector2 rotateHelp(Vector2 v, float delta) {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

}
