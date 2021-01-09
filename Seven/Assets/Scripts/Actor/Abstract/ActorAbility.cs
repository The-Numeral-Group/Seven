using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

/*public abstract class ActorAbility : MonoBehaviour
{
    public string title;                //This is a good idea we should keep it (as abilityName)
    public abstract void DoAbility();   //This translates into/is split into Invoke and InternInvoke
}*/
/*ActorAbility is split between two Abstract classes to make things easy for
individuals needing to hold ActorAbilities. 

We want the types an ability takes and returns to be changable between abilities, 
so we need to make ActorAbilityFunction generic. We add the ActorAbility class on top
of that to make sure that individuals who need to hold ability references don't need to 
specify type.*/

/*This abstract class makes sure that every public method in ActorAbilityFunction
is public while also hiding the generic types ActorAbilityFunction uses*/
public abstract class ActorAbility : MonoBehaviour
{
    public abstract bool getUsable();
    public abstract IEnumerator coolDown(float cooldownDuration);
    public abstract void Invoke(ref Actor user);
}

public abstract class ActorAbilityFunction<InvokeParam, InvokeReturn> : ActorAbility
{
    //public fields
    public float cooldownPeriod;
    public string abilityName;

    /*private with accessor so others can look without changing
    yes it could be a property but I (Thomas) can't figure
    how to do read-only properties that can be changed within
    the class*/
    //Update: Now usable is protected.
    protected bool usable = true;
    public override bool getUsable(){return usable;}

    /*IEnumerator for using coroutines to run ability cooldowns.
    If you don't want a cooldown, pass in 0.0f*/
    public override IEnumerator coolDown(float cooldownDuration)
    {
        usable = false;
        yield return new WaitForSeconds(cooldownDuration);
        usable = true;
    }

    /*The method that outsiders will use to actually start the ability.
    It takes the ability's user as an argument, as an ActorAbility object is
    never expected to always knows what that is. 
    
    We could turn this into a Unity Message call by removing the argument, but
    that means we'd need InternInvoke to blindly figure out the ability's user,
    and everything else, which I (Thomas) think is too much trouble.
    
    The ref keyword is used here to allow Invoke to make changes directly
    to the user object. C# Objects are passed to functions by reference anyways,
    but it's the thought that counts.*/
    public override void Invoke(ref Actor user)
    {
        //by default, Invoke just does InternInvoke with no arguments
        if(usable)
        {
            InternInvoke(new InvokeParam[0]);
            StartCoroutine(coolDown(cooldownPeriod));
        }
        
    }

    /*The only abstract method. The abilities should do their actual abilitying
    within this method. This method can honestly do whatever it wants.*/
    protected abstract InvokeReturn InternInvoke(params InvokeParam[] args);
}