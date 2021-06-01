using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathP2AbilityInitiator : ActorAbilityInitiator
{
    public ActorAbility wrathArmSweep;
    public WrathShockwave wrathShockwave;
    public WrathFireBrimstone wrathFireBrimstone;
    public WrathEnraged wrathEnraged;

    void Awake()
    {
        this.abilities.Add("Wrath" + nameof(wrathArmSweep), wrathArmSweep);
        AbilityRegister.WRATH_ARMSWEEP = "Wrath" + nameof(wrathArmSweep);

        this.abilities.Add("Wrath" + nameof(wrathShockwave), wrathShockwave);
        AbilityRegister.WRATH_SHOCKWAVE = "Wrath" + nameof(wrathShockwave);

        this.abilities.Add("Wrath" + nameof(wrathFireBrimstone), wrathFireBrimstone);
        AbilityRegister.WRATH_FIREBRIMSTONE = "Wrath" + nameof(wrathFireBrimstone);

        this.abilities.Add("Wrath" + nameof(wrathEnraged), wrathEnraged);
        AbilityRegister.WRATH_ENRAGED = "Wrath" + nameof(wrathEnraged);
    }
}
