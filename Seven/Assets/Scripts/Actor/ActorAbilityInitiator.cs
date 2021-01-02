//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
//SerializableDictionary<> is not in project yet, get it from here:
//https://assetstore.unity.com/packages/tools/utilities/serialized-dictionary-lite-110992
using RotaryHeart.Lib.SerializableDictionary;

public class ActorAbilityInitiator : MonoBehaviour
{
    
    public abilityDict abilities; //{ get; private set;}
    
    void Awake(){
        //this.abilities = new abilityDict();
    }

    public virtual void DoAttack()
    {
        //by default this does nothing
    }
}

public class abilityDict : SerializableDictionaryBase<string, ActorAbility>{}
