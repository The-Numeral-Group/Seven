using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrideAbilityInitiator : ActorAbilityInitiator
{
    //FIELDS---------------------------------------------------------------------------------------
    public ActorAbility Close;
    public ActorAbility Far;
    public ActorAbility Special;
    //METHODS--------------------------------------------------------------------------------------
    void Awake()
    {
        //Manual initialization, 'cause I (Thomas) can't figure out a smarter way.

        this.abilities.Add("Pride" + nameof(Close), Close);
        AbilityRegister.PRIDE_CLOSE_ATTACK = "Pride" + nameof(Close);

        this.abilities.Add("Pride" + nameof(Far), Far);
        AbilityRegister.PRIDE_FAR_ATTACK = "Pride" + nameof(Far);

        this.abilities.Add("Pride" + nameof(Special), Special);
        AbilityRegister.PRIDE_SPECIAL = "Pride" + nameof(Special);
    }
}
