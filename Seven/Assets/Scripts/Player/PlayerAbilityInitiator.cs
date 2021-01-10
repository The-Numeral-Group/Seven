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
    
    /*// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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

    }

    public void DoDodge()
    {

    }
}
