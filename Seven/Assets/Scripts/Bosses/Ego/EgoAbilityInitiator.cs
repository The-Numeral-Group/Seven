

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EgoAbilityInitiator : ActorAbilityInitiator
{
    //FIELDS---------------------------------------------------------------------------------------
    public ActorAbility Beam;
    public ActorAbility Wall;
    public ActorAbility Special;
    //METHODS--------------------------------------------------------------------------------------
    void Awake()
    {
        //Manual initialization, 'cause I (Thomas) can't figure out a smarter way.

        this.abilities.Add("Ego2" + nameof(Beam), Beam);
        AbilityRegister.EGO_BEAM = "Ego2" + nameof(Beam);

        this.abilities.Add("Ego2" + nameof(Wall), Wall);
        AbilityRegister.EGO_WALL = "Ego2" + nameof(Wall);

        this.abilities.Add("Ego2" + nameof(Special), Special);
        AbilityRegister.EGO_SUMMON = "Ego2" + nameof(Special);
    }
}