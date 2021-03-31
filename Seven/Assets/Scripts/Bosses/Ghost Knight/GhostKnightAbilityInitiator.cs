using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Doc: https://docs.google.com/document/d/1pn5i67V0rqvt_sF2F92N-yjq4jcTOx8UHNk8indCvps/edit
public class GhostKnightAbilityInitiator : ActorAbilityInitiator
{
    public ActorAbility Slash;
    public ActorAbility Projectile;
    public ActorAbility Special;

    void Awake()
    {
        this.abilities.Add("GhostKnight" + nameof(Slash), Slash);
        AbilityRegister.GHOSTKNIGHT_SLASH = "GhostKnight" + nameof(Slash);

        this.abilities.Add("GhostKnight" + nameof(Projectile), Projectile);
        AbilityRegister.GHOSTKNIGHT_PROJECTILE = "GhostKnight" + nameof(Projectile);

        this.abilities.Add("GhostKnight" + nameof(Special), Special);
        AbilityRegister.GHOSTKNIGHT_SPECIAL = "GhostKnight" + nameof(Special);

    }
}
