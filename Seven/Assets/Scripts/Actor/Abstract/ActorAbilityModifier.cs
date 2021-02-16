using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using changeDict = System.Collections.Generic.Dictionary<string, System.Action<dynamic>>;

public abstract class ActorAbilityModifier
{
    //FIELDS---------------------------------------------------------------------------------------
    //the dictionary that holds all of the desired changes, stored on a member-by-member basis
    private changeDict changes;

    //the dictionary that holds all of the desired function calls, stored on a '
    //method-by-method basis
    //private changeDict calls;

    //CONSTRUCTORS---------------------------------------------------------------------------------
    public ActorAbilityModifier()
    {
        //create the dictionaries
        changes = new changeDict();
        //calls = new changeDict();

        //and fill them with the desired changes
        InitializeChanges(changes);
    }
    
    //METHODS--------------------------------------------------------------------------------------
    /*Add all of the changes you want to make here by adding them as entries to the changeDict.
    Each change should be keyed to the member being used, and the value should be a delegate that
    accesses the member. If you need multiple members to use 1 member in 1 Action delegate, use 
    ActorAbilityModifier.DoesMemberExist.*/
    protected abstract void InitializeChanges(changeDict changes);

    /*//Exact same as above, but with method calling instead of field modification
    protected virtual void InitializeCalls(changeDict calls)
    {
        return;
    }*/

    /*Changes will be exectuted here. This method is sealed (that is, it can't be overriden) 
    because it does not to be overriden. Uh... at least, I (Thomas) think. The point is if 
    you want more granular control, do it in your delegates. Unsealing this method should 
    be discussed first, because this method NEEDS to be completely type-safe at all times.*/
    public void ModifyAbility(ActorAbility ability)
    {
        //convert the ability to a dynamic. This is the equivilant of telling the compiler
        //  "bro, trust me"
        dynamic dyn = ability;

        //technically change.Keys isn't a string, but the conversion is implicit. Anyways...
        /*Loop through all of the members in the changeDict's keys */
        foreach(string member in changes.Keys)
        {
            /*If the ability has that member, invoke the change/method call associated
            with the key. If it doesn't, do nothing.*/
            if(ActorAbilityModifier.DoesMemberExist(dyn, member))
            {
                //the change happens here
                changes[member]?.Invoke(dyn);
            }
        }
    }

    //Exact same as above, but with method calling instead of field modification
    /*public sealed void CallAbility(ActorAbility ability)
    {
        
    }*/

    //DoesMemberExist

    /*Checks if a given dynamic object has a member (properties, methods, fields, events, and so on)
    of the listed name.*/
    protected static bool DoesMemberExist(dynamic dyn, string memberName)
    {
        //fun fact this could all be done in one line lol
        MemberInfo[] members = ((object)dyn).GetType().GetMembers();
        MemberInfo memberPresent = Array.Find(members, (MemberInfo memberInfo) => 
            {
                return String.Equals(memberInfo.Name, memberName);
            }
        );
        return memberPresent != null; 
    }
}
