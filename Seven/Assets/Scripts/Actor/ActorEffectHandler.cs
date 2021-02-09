using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*ActorEffectHandler does need an actor, but if you make
two components require each other, you can't remove either in
the inspector, which is a massive pain.*/
//[RequireComponent(typeof(Actor))]
public class ActorEffectHandler : MonoBehaviour
{
    private Actor hostActor;
    [SerializeField]private List<ActorEffect> appliedEffects;
    //is ActorEffect a class? do we pass in a bunch of actoreffect compoenents?
    //Example do gluttony's particles contian a script for this effect? 
    //do we then have the effect script tied to the particle and it gets added to the players effect handler?
    //                                  v
    /*ActorEffect is an interface, so we pass in whatever objects/classes implement it.
    This means that each effect needs its own script, but where it's attached doesn't really
    matter.
    For example (as of 12/28/20) Gluttony's sin effect is handled in GluttonySin.cs, which implements
    the interface. That same script also handles collisions with the player and other functionality, because
    ActorEffectHandler ignores all other functions and only looks at the ActorEffect functions.*/

    private void Awake()
    {
        //This is just here because it's more technically correct
        //The compiler wouldn't have minded if it was above like before
        appliedEffects = new List<ActorEffect>();
    }

    private void Start()
    {
        /*This needs to go in Start because
        components might not be instantiated by the
        time awake is called.*/
        hostActor = this.GetComponent<Actor>();
    }

    public void AddEffect(ActorEffect effect)
    {
        /*ActorEffectHandler needs to apply the effect and then
        check if the application was successful before adding it
        to appliedEffects*/
        bool successfullyApplied = effect.ApplyEffect(ref hostActor);
        if(successfullyApplied)
        {
            appliedEffects.Add(effect);
        }
    }

    public void AddTimedEffect(ActorEffect effect, float time)
    {
        StartCoroutine(EffectTimer(effect, time));
    }

    //Renamed from RemoveEffect to SubtractEffect to avoid being 
    //confused with ActorEffect.RemoveEffect()
    public void SubtractEffect(ActorEffect effect)
    {
        if(appliedEffects.Contains(effect))
        {
            effect.RemoveEffect(ref hostActor);
            appliedEffects.Remove(effect);
        }
        else
        {
            Debug.LogWarning("ActorEffectHandler attempted to Subtract an effect it did not have");
        }
        
    }

    //Returns whether or not an effect of the specified type is present in appliedEffects
    public bool EffectPresent<TypeToLocate>()
    {
        return appliedEffects.Find(effect => effect is TypeToLocate) != null;
    }

    //Returns the number of instances of an effect of the specified type  present in appliedEffects
    public int EffectPresentCount<TypeToLocate>()
    {
        return appliedEffects.FindAll(effect => effect is TypeToLocate).Count;
    }

    public void SubtractEffectByType<TypeToSubtract>()
    {
        //Does RemoveEffectByType remove the first instance of a typed effect? how does it choose?
        //                                  v
        /*It will remove the first one it finds. Because we only use Add function (which puts things
        on the back of the list) to add effects to the list, this will also be the eldest effect
        of that type*/

        /*appliedEffects.Find() takes a predicate (basically a special kind of C# function pointer)
        and locates the first item in appliedEffects that makes it (effect is TypeToSubtract) 
        return true, or null otherwise. That is then passed into the normal SubtractEffect. */
        SubtractEffect(appliedEffects.Find(effect => effect is TypeToSubtract));
        
    }

    public void SubtractAllEffectsByType<TypeToSubtract>()
    {
        List<ActorEffect> subtractedEffects = appliedEffects.FindAll(effect => effect is TypeToSubtract);
        while(subtractedEffects.Count != 0){
            //grab the first effect (this is arbitrary)
            ActorEffect effect = subtractedEffects[0];
            //subtract it from appliedEffects
            SubtractEffect(effect);
            //remove it from the sublist
            subtractedEffects.Remove(effect);
        }
        //both of these methods would be cleaner to use, but can't for reasons explained below
        //subtractedEffects.ForEach(effect => SubtractEffect(effect));
        //appliedEffects.FindAll(effect => effect is TypeToSubtract).ForEach(effect => SubtractEffect(effect));
    }

    public void SubtractAllEffects()
    {
        while(appliedEffects.Count != 0){
            //grab the first effect (this is arbitrary)
            ActorEffect effect = appliedEffects[0];
            //subtract it from appliedEffects
            SubtractEffect(effect);
        }
        /*
        foreach (ActorEffect effect in appliedEffects)
        {
            effect.StopEffect();
        }
        */
        //appliedEffects.Clear(); //Regular list clear but do the effect scripts need to be deactivated?
        //                                  v
        /*Yeah, each effect needs to be deactivated manually. Also, we can't actually use a foreach loop
        to do mass deactivation because the enumerator (i.e. the list) used in the loop can't be changed
        until the loop is finished. Since deactivating an effect involves removing it from that list
        and potentially destroying the effect object, it can't be used.*/
    }

    IEnumerator EffectTimer(ActorEffect effect, float time)
    {
        AddEffect(effect);
        yield return new WaitForSeconds(time);
        /*we premptively check if effect has ceased to exist, just in case the effect can be destroyed
        before its timer is over. In this case, there's no error, so skip this step.*/
        if(effect != null){
            SubtractEffect(effect);
        }
    }
}
