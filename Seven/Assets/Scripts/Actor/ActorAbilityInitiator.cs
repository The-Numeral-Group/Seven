//using System.Collections;
//using System.Collections.Generic;
using System;
using UnityEngine;

public class ActorAbilityInitiator : MonoBehaviour
{
    public abilityDict abilities;//{ get; private set;}
    
    void Awake(){
        //this.abilities = new abilityDict();
    }

    public virtual void DoAttack()
    {
        //by default this does nothing
    }
}

 [Serializable]
 public class abilityDict : SerializableDictionary<string, ActorAbility> {}
