using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CsharpRandom = System.Random;
using WeightTuple = System.Tuple<WeaponAbility, int>;

public class BranchingWeaponAbility : WeaponAbility
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("Whether or not this BWA should wait for the selected ability to cooldown" + 
        " before trying to use it. If set to false, weapons must be off cooldown to be chosen.")]
    public bool waitForCooldowns = false;

    [Tooltip("The weapons that can potentailly be used when this weapon is invoked.")]
    public List<WeaponAbility> potentialWeps;

    [Tooltip("The weights of each weapon being chosen (higher number = higher relative odds." + 
        " Applied to potentialWeps in a 1-to-1 order.")]
    public List<int> wepWeights;

    [Tooltip("The weight to give to weapons that don't have weights")]
    public int defaultWeight = 5;

    //the random number generator that determines which weapon gets used
    private CsharpRandom rand;

    //the pairs of weapons and weights
    private List<WeightTuple> wepPairs;

    //the biggest number in wepWeights
    private int largestWeight = 0;

    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    protected override void Start()
    {
        rand = new CsharpRandom();
        wepPairs = new List<WeightTuple>();

        //pair up the weapons and the weights together for ease 
        //also save the biggest weight, for range constructions
        for(int i = 0; i < potentialWeps.Count; ++i)
        {
            if(i >= wepWeights.Count)
            {
                wepPairs.Add(new WeightTuple(potentialWeps[i], defaultWeight));
                largestWeight = defaultWeight > largestWeight ? defaultWeight : largestWeight;
            }
            else
            {
                wepPairs.Add(new WeightTuple(potentialWeps[i], wepWeights[i]));
                largestWeight = wepWeights[i] > largestWeight ? wepWeights[i] : largestWeight;
            }
        }
    }
    //Wrapper for coroutine-based timing function
    protected override int InternInvoke(params Actor[] args)
    {
        if(potentialWeps.Count == 0)
        {
            Debug.LogError("BranchWeaponAbility: no weapons provided!");
            return 1;
        }

        StartCoroutine(PickAbility(args));
        return 0;
    }

    /*Randomly selects a weapon ability to use, invokes it, and then waits for it to finish*/
    IEnumerator PickAbility(params Actor[] args)
    {
        //pick a random from 0 to the largestWeight - 1
        int opt = rand.Next(0, largestWeight);

        //determine the smallest item in weights that is larger than opt
        int winner = largestWeight;
        foreach(WeightTuple tupl in wepPairs)
        {
            bool cooldownClear = waitForCooldowns || tupl.Item1.getUsable();
            if(tupl.Item2 > opt && tupl.Item2 < winner && cooldownClear)
            {
                winner = tupl.Item2;
            }
        }

        //find all tuples with that weight
        List<WeightTuple> winners = wepPairs.FindAll( (tuple) => {return tuple.Item2 == largestWeight;} );

        //pick a random from 0 to the winners.Count - 1
        var tie = rand.Next(0, winners.Count);
        
        //use that weapon
        WeaponAbility nextWep = winners[tie].Item1;
        Debug.Log($"BranchingWeaponAbility: Selected {opt}. Choosing Item: {nextWep.name}, {winner}");

        //if we're waiting on cooldowns...
        if(waitForCooldowns)
        {
            //pause for the cooldown
            yield return new WaitUntil( () => nextWep.getIsFinished() );
        }

        //invoke that weapon
        nextWep.Invoke(ref user, args);

        //wait for it to end
        yield return new WaitUntil( () => nextWep.getIsFinished());

        Debug.Log("BranchingWeaponAbility: Branch done!");
        this.isFinished = true;
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
