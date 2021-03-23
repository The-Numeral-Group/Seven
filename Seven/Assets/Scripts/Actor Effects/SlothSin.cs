//Sloth's sin has no effect (3/22/21), but there's nothing unique to pin it to, so it's just its
//own file right now.
public class SlothSin : ActorEffect
{
    public SlothSin(){}

    //Currently, SlothSin has no actual effect on the player
    public bool ApplyEffect(ref Actor actor)
    {
        return true;
    }

    //Currently, SlothSin does nothing
    public void RemoveEffect(ref Actor actor)
    {

    }
}
