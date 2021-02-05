using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GluttonyAbilityInitiatorP2 : ActorAbilityInitiator
{
    public ActorAbility Projectile;
    public ActorAbility PhaseTwoSpecial;
    void Awake()
    {
        this.abilities.Add("Gluttony" + nameof(Projectile), Projectile);
        AbilityRegister.GLUTTONY_PHASETWO_PROJECTILE = "Gluttony" + nameof(Projectile);

        this.abilities.Add("Gluttony" + nameof(PhaseTwoSpecial), PhaseTwoSpecial);
        AbilityRegister.GLUTTONY_PHASETWO_SPECIAL = "Gluttony" + nameof(PhaseTwoSpecial);
    }
}
