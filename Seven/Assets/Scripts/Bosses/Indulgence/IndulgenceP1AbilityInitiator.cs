﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndulgenceP1AbilityInitiator : ActorAbilityInitiator
{
    public IndulgenceCrush crush;
    public IndulgenceWallCrawl wallCrawl;
    public IndulgenceLegAttack legAttack;
    void Awake()
    {
        this.abilities.Add("Indulgence" + nameof(crush), crush);
        AbilityRegister.INDULGENCE_CRUSH = "Indulgence" + nameof(crush);

        this.abilities.Add("Indulgence" + nameof(wallCrawl), wallCrawl);
        AbilityRegister.INDULGENCE_WALLCRAWL = "Indulgence" + nameof(wallCrawl);

        this.abilities.Add("Indulgence" + nameof(legAttack), legAttack);
        AbilityRegister.INDULGENCE_PHYSICAL = "Indulgence" + nameof(legAttack);
    }
}
