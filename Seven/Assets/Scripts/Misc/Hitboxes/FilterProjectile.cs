using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterProjectile : BasicProjectile
{
    //FIELDS---------------------------------------------------------------------------------------
    //the gameobject this projectile will try to collide with. All others will be ignored
    protected GameObject targetObj = null;

    //METHODS--------------------------------------------------------------------------------------
    //What happens when the projectile actually hits something
    //ignore the collision if what was hit wasn't the target
    protected override void OnTriggerEnter2D(Collider2D collided)
    {
        if(!targetObj || targetObj == collided.gameObject)
        {
            base.OnTriggerEnter2D(collided);
        }
    }

    /*Starts the projectile!
    Also saves a target, if any. This object will only collide with that target*/
    public virtual void Launch(Vector2 target, LAUNCH_MODE mode = LAUNCH_MODE.POINT,
        GameObject targetObj = null)
    {
        this.targetObj = targetObj;

        base.Launch(target, mode);
    }
}
