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
    public abstract bool getIsFinished();
    public abstract IEnumerator coolDown(float cooldownDuration);
    public abstract void Invoke(ref Actor user);
    public abstract void Invoke(ref Actor user, params object[] args);
}

public abstract class ActorAbilityFunction<InvokeParam, InvokeReturn> : ActorAbility
{
    //public fields
    //Handles the cooldown of an ability
    [Tooltip("How long an ability will be on cooldown after its' use.")]
    public float cooldownPeriod;
    //Name for the ability
    [Tooltip("Name of the ability. Currently does nothing.")]
    public string abilityName;

    [Tooltip("Whether this ability should try to execute even if" + 
        " it can't use the provided arguments")]
    public bool invokeBlankWhenBadArguments = true;

    /*private with accessor so others can look without changing
    yes it could be a property but I (Thomas) can't figure
    how to do read-only properties that can be changed within
    the class*/
    //Update: Now usable is protected.
    protected bool usable = true;
    public override bool getUsable(){return usable;}

    /*private accessors much like usable
    isFinished is used to signify if an ability is in use.
    Invoke by default should set it, it is the abilities responsibility
    reset it.*/
    protected bool isFinished = true;
    public override bool getIsFinished(){return isFinished;}

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
            isFinished = false;
            InternInvoke(new InvokeParam[0]);
            StartCoroutine(coolDown(cooldownPeriod));
        }
        
    }

    /*Same as the above method, but this overload allows an arbitrary number of
    Object objects to be passed in to assist the ability. It's up to the ability
    to figure out what to do with these additional arguments, and what to do if
    it gets something it doesn't expect.*/
    public override void Invoke(ref Actor user, params object[] args)
    {
        //by default, Invoke just does InternInvoke with the provided arguments
        //it's also just implicitly convert the args and give it to InternInvoke
        if(usable)
        {
            isFinished = false;
            InternInvoke(easyArgConvert(args));
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    /*The only abstract method. The abilities should do their actual abilitying
    within this method. This method can honestly do whatever it wants.*/
    protected abstract InvokeReturn InternInvoke(params InvokeParam[] args);

    /*Just a wrapper for Array.ConvertAll, since it has a really long signature*/
    InvokeParam[] easyArgConvert(object[] inputArray)
    {
        return System.Array.ConvertAll<object, InvokeParam>(inputArray, (object x) => 
            {
                if(x is InvokeParam y)
                {
                    return y;
                }
                else
                {
                    return default(InvokeParam);
                }
            }
        );
    }
}