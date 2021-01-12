﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityInitiator : ActorAbilityInitiator
{
    /*these are the two abilities that the player sees on their UI.*/
    public ActorAbility selectedAbilityAlpha;
    public ActorAbility selectedAbilityBeta;

    /*This is the player's implicit dodge. Switch this out
    to change how the player dodges*/
    public ActorAbility playerDodge;

    /*This is the player's implicit attaack. Switch this out
    to change how the player attacks*/
    public ActorAbility playerAttack;

    //Don't know if this is needed, but just using the player actor to pass by ref to the invoke for attack and dodge.
    //It is, and every actor probably needs it since each ability needs their user. I've added it to
    //  ActorAbilityInitiator, so now it's innate to the class -Thomas
    //public Actor playerActor;

    /*// Update is called once per frame
    void Update()
    {
        
    }*/

    //this is the method called by an input press
    public void OnAttack()
    {
        DoAttack();
    }

    public override void DoAttack()
    {
        //this.gameObject.SendMessage("DoWeaponAttack");
        playerAttack.Invoke(ref userActor);
    }

    public void OnDodge()
    {
        DoDodge();
    }

    public void DoDodge()
    {
        playerDodge.Invoke(ref userActor);
    }
}