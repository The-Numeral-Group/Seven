using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndulgenceP2AbilityInitiator : ActorAbilityInitiator
{
    public IndulgenceCharge charge;
    public IndulgenceP1ProjAttack projAttack;
    public IndulgenceSpecial special;
    void Awake()
    {
        this.abilities.Add("Indulgence" + nameof(charge), charge);
        AbilityRegister.INDULGENCE_CHARGE = "Indulgence" + nameof(charge);

        this.abilities.Add("Indulgence" + nameof(projAttack), projAttack);
        AbilityRegister.INDULGENCE_PROJECTILE = "Indulgence" + nameof(projAttack);

        this.abilities.Add("Indulgence" + nameof(special), special);
        AbilityRegister.INDULGENCE_SPECIAL = "Indulgence" + nameof(special);
    }
}
