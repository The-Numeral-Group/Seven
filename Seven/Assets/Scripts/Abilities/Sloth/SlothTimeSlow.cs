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

    //the actual amount by which time will be effected
    private float trueTimeChange;
    //METHODS--------------------------------------------------------------------------------------
    //Start is called on the first frame this object is active
    void Awake()
    {
        trueTimeChange = Time.timeScale * (-timeEffect);
    }

    //The actual function of the ability
    protected override int InternInvoke(params int[] args)
    {
        if(clock != null)
        {
            ///DEBUG
            Debug.Log("SlothTimeSlow: slow occurring...");
            ///DEBUG
            clock.ForceTimedTimeApplication(trueTimeChange, timeDuration);
            return 1;
        }
        else
        {
            Debug.LogWarning("SlothTimeSlow: No defined SlothClockTimeWarp, cannot slow down time");
        }

        return 0;
    }
}
