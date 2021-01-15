using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GluttonyAbilityInitiator : ActorAbilityInitiator
{
    public ActorAbility Crush;
    public ActorAbility Bite;
    public ActorAbility PhaseZeroSpecial;
    public ActorAbility Projectile;

    void Awake()
    {
        //Manual initialization, 'cause I (Thomas) am lazy

        this.abilities.Add("Gluttony" + nameof(Crush), Crush);
        AbilityRegister.GLUTTONY_CRUSH = "Gluttony" + nameof(Crush);

        this.abilities.Add("Gluttony" + nameof(Bite), Bite);
        AbilityRegister.GLUTTONY_BITE = "Gluttony" + nameof(Crush);

        this.abilities.Add("Gluttony" + nameof(PhaseZeroSpecial), PhaseZeroSpecial);
        AbilityRegister.GLUTTONY_PHASEZERO_SPECIAL = "Gluttony" + nameof(PhaseZeroSpecial);

        this.abilities.Add("Gluttony" + nameof(Projectile), Projectile);
        AbilityRegister.GLUTTONY_PROJECTILE = "Gluttony" + nameof(Projectile);
    }
}
