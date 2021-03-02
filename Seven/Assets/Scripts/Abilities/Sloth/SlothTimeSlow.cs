using UnityEngine;

public class SlothTimeSlow : ActorAbilityFunction<int, int>
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("By what percentage time will be slowed when this ability activates.")]
    public float timeEffect = 0.25f;

    [Tooltip("How long the time slow will last")]
    public float timeDuration = 6f;

    [Tooltip("The slow-down clock this ability will use to force the time slow.")]
    public SlothClockTimeWarp clock;
    //METHODS--------------------------------------------------------------------------------------
    protected override int InternInvoke(params int[] args)
    {
        if(clock != null)
        {
            ///DEBUG
            Debug.Log("SlothTimeSlow: slow occurring...");
            ///DEBUG
            clock.ForceTimedTimeApplication(timeEffect, timeDuration);
            return 1;
        }
        else
        {
            Debug.LogWarning("SlothTimeSlow: No defined SlothClockTimeWarp, cannot slow down time");
        }

        return 0;
    }
}
