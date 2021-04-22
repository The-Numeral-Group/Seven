using System.Collections;
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

    /*This is the player's implicit interact. Switch this out
    to change how the player interacts*/
    public ActorAbility playerInteract;

    void Awake()
    {
        //Manual initialization, 'cause I (Thomas) have come to realize it can't be done automatically

        this.abilities.Add("Player" + nameof(selectedAbilityAlpha), selectedAbilityAlpha);
        AbilityRegister.PLAYER_SELECTED_A = "Player" + nameof(selectedAbilityAlpha);

        this.abilities.Add("Player" + nameof(selectedAbilityBeta), selectedAbilityBeta);
        AbilityRegister.PLAYER_SELECTED_B = "Player" + nameof(selectedAbilityBeta);

        this.abilities.Add("" + nameof(playerAttack), playerAttack);
        AbilityRegister.PLAYER_ATTACK = "" + nameof(playerAttack);

        this.abilities.Add("" + nameof(playerDodge), playerDodge);
        AbilityRegister.PLAYER_DODGE = "" + nameof(playerDodge);

        this.abilities.Add("" + nameof(playerInteract), playerInteract);
        AbilityRegister.PLAYER_INTERACT = "" + nameof(playerInteract);
    }

    //Don't know if this is needed, but just using the player actor to pass by ref to the invoke for attack and dodge.
    //It is, and every actor probably needs it since each ability needs their user. I've added it to
    //  ActorAbilityInitiator, so now it's innate to the class -Thomas
    //public Actor playerActor;

    /* Update is called once per frame
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

    public void OnInteract()
    {
        DoInteract();
    }

    public void DoInteract()
    {
        playerInteract.Invoke(ref userActor);
    }

    public void OnAbilityOne()
    {
        if(selectedAbilityAlpha)
        {
            selectedAbilityAlpha.Invoke(ref userActor);
        }
    }

    public void OnAbilityTwo()
    {
        if (selectedAbilityBeta)
        {
            selectedAbilityBeta.Invoke(ref userActor);
        }
    }
}
