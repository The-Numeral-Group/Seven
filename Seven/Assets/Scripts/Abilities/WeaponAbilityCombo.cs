﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAbilityCombo : ActorAbilityFunction<int, int>
{
    [Tooltip("Set to true to abort the combo if an ability is on cooldown when it needs to" + 
        " be used in the combo.")]
    public bool cooldownCancel = false;

    [Tooltip("The WeaponAbilites used in this combo. They will be executed in order.")]
    public List<WeaponAbility> comboList;
    
    //Begins the combo by starting a coroutine
    protected override int InternInvoke(params int[] args)
    {
        usable = false;
        StartCoroutine(ComboInvokation());
        return 0;
    }

    /*The actual combo. Invokes each WeaponAbility in comboList in order,
    waiting for each one to complete and aborting if an ability is on cooldown
    when it comes up.*/
    IEnumerator ComboInvokation()
    {
        /*In some earlier code I avoided using foreach because I thought it wasn't order
        sensitive. In reality, the order depends on how the enumeration for the particular
        class is implemented. For List<T>, it's in order.*/
        foreach(WeaponAbility comboPiece in comboList)
        {
            //pause for a moment so the user can think between attacks
            yield return null;

            //If the weapon is on cooldown AND it should stop the combo if it's not available...
            if(!comboPiece.getUsable() && cooldownCancel)
            {
                //end this loop early
                break;
            }

            //invoke the comboPiece with no argument (as weapon abilities do)
            comboPiece.Invoke(ref user);

            //wait for it to finish
            yield return new WaitUntil( () => comboPiece.getIsFinished());
        }

        //We're done now. Time to cooldown!
        StartCoroutine(coolDown(cooldownPeriod));
    }
}
