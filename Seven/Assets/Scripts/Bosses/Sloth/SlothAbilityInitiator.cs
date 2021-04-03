using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlothAbilityInitiator : ActorAbilityInitiator
{
    //FIELDS---------------------------------------------------------------------------------------
    public ActorAbility Close;
    public ActorAbility Far;
    public ActorAbility Special;
    //METHODS--------------------------------------------------------------------------------------
    void Awake()
    {
        //Manual initialization, 'cause I (Thomas) can't figure out a smarter way.

        this.abilities.Add("Sloth" + nameof(Close), Close);
        AbilityRegister.SLOTH_PHYSICAL = "Sloth" + nameof(Close);

        this.abilities.Add("Sloth" + nameof(Far), Far);
        AbilityRegister.SLOTH_RANGE = "Sloth" + nameof(Far);

        this.abilities.Add("Sloth" + nameof(Special), Special);
        AbilityRegister.SLOTH_SPECIAL = "Sloth" + nameof(Special);
    }
}
