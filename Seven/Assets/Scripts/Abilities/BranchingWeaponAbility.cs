﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CsharpRandom = System.Random;

public class BranchingWeaponAbility : WeaponAbility
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("The weapons that can potentailly be used when this weapon is invoked.")]
    public List<WeaponAbility> potentialWeps;

    //the random number generator that determines which weapon gets used
    private CsharpRandom rand;

    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    protected override void Start()
    {
        rand = new CsharpRandom();

    }
    //Wrapper for coroutine-based timing function
    protected override int InternInvoke(params Actor[] args)
    {
        if(potentialWeps.Count == 0)
        {
            Debug.LogError("BranchWeaponAbility: no weapons provided!");
            return 1;
        }

        StartCoroutine(PickAbility());
        return 0;
    }

    /*Randomly selects a weapon ability to use, invokes it, and then waits for it to finish*/
    IEnumerator PickAbility()
    {
        //pick a weapon to use by randomly choosing an index value
        ///DEBUG
        int opt = rand.Next(potentialWeps.Count);
        ///DEBUG
        var nextWep = potentialWeps[opt];
        Debug.Log($"BranchingWeaponAbility: Choosing Item {opt}: {nextWep.name}");

        //invoke that weapon
        nextWep.Invoke(ref user);

        //wait for it to end
        yield return new WaitUntil( () => nextWep.getIsFinished());
    }

    /*none of these methods are actually doing anything, they are simply overriden here
    to accurately hide functionality for the wrapper*/
    protected override void Awake(){}
    protected override void SpawnWeapon(params Actor[] args){}
    public new IEnumerator SheatheWeapon()
    {
        yield return null;
    }

}