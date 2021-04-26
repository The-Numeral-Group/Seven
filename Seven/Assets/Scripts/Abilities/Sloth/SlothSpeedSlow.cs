using UnityEngine;

public class SlothSpeedSlow : ActorAbilityFunction<int, int>
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("By what percentage the target will be slowed when this ability activates.")]
    public float speedEffect = 0.25f;

    [Tooltip("The slow-down clock this ability will use to force the time slow.")]
    public SlothClockSpeedWarp clock;

    //the actual amount by which time will be effected
    private float trueSpeedChange;
    //METHODS--------------------------------------------------------------------------------------
    //Start is called on the first frame this object is active
    void Start()
    {
        clock.slowFactor = speedEffect;
    }

    //The actual function of the ability
    protected override int InternInvoke(params int[] args)
    {
        if(clock != null)
        {
            ///DEBUG
            Debug.Log("SlothSpeedSlow: slow occurring...");
            ///DEBUG
            clock.ForceSlow();
            return 1;
        }
        else
        {
            Debug.LogWarning("SlothSpeedSlow: No defined SlothClockSpeedWarp, cannot" +
                " slow down target");
        }

        return 0;
    }
}
