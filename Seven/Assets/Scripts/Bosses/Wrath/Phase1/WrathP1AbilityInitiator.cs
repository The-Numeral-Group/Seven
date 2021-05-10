using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathP1AbilityInitiator : ActorAbilityInitiator
{
    public WrathChainPull chainPull;
    public WrathFireWall fireWall;
    public WrathSludge sludge;
    public WrathSwordAttack swordAttack;
    public WrathSwordRush swordRush;

    void Awake()
    {
        this.abilities.Add("Wrath" + nameof(chainPull), chainPull);
        AbilityRegister.WRATH_CHAINPULL = "Wrath" + nameof(chainPull);

        this.abilities.Add("Wrath" + nameof(fireWall), fireWall);
        AbilityRegister.WRATH_FIREWALL = "Wrath" + nameof(fireWall);

        this.abilities.Add("Wrath" + nameof(sludge), sludge);
        AbilityRegister.WRATH_SLUDGE = "Wrath" + nameof(sludge);

        this.abilities.Add("Wrath" + nameof(swordAttack), swordAttack);
        AbilityRegister.WRATH_SWORDATTACK = "Wrath" + nameof(swordAttack);

        this.abilities.Add("Wrath" + nameof(swordRush), swordRush);
        AbilityRegister.WRATH_SWORDRUSH = "Wrath" + nameof(swordRush);
    }
}
