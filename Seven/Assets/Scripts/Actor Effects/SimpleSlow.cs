public class SimpleSlow : ActorEffect
{
    //FIELDS---------------------------------------------------------------------------------------
    //[Tooltip("How many units slower the afflicted Actor should go (making this number" + 
    //    " negative will make the Actor go faster).")]
    public float slowAmount;

    //the amount of speed that was removed. This is tracked so it can be properly reverted
    //  in the case this effect applies an incomplete slow (such as if the slow would drop the
    //  Actor's speed below 0, making them move backwards.)
    private float appliedSlowAmount = 0.0f;

    //CONSTRUCTORS---------------------------------------------------------------------------------
    /*I (Thomas) realized this didn't need to be a Monobehaviour, so I added a constructor and
    took away that superclass so SimpleSlows can just be pulled out of thin air, as needed.*/
    public SimpleSlow(float slowAmount)
    {
        this.slowAmount = slowAmount;
    }

    //METHODS--------------------------------------------------------------------------------------
    public bool ApplyEffect(ref Actor actor)
    {
        var movement = actor.myMovement;
        if(movement.speed > slowAmount)
        {
            appliedSlowAmount = slowAmount;
            movement.speed -= appliedSlowAmount;
            return true;
        }
        else if(movement.speed > 0)
        {
            appliedSlowAmount = movement.speed;
            movement.speed = 0;
            return true;
        }

        //if the code gets here, that means that the Actor's speed is 0 or less.
        UnityEngine.Debug.LogWarning("SimpleSlow: Effect can't be applied if it would make" + 
            " target speed negative");
        return false;
    }
    public void RemoveEffect(ref Actor actor)
    {
        actor.myMovement.speed += appliedSlowAmount;
    }
    
}
