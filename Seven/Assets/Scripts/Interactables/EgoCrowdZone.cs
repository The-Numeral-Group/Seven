using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EgoCrowdZone : Interactable
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("By what factor the user's speed should be multiplied.")]
    public float boostFactor = 2f;

    [Tooltip("How long the effect should last.")]
    public float duration = 5f;

    [Tooltip("The material to apply to whoever recieves the spotlight.")]
    public Material effectMaterial;

    [Tooltip("How long the player should flex for when interacting with the spotlight")]
    public float flexTime = 0.25f;

    //reference to the player
    private Actor player;
    //METHODS--------------------------------------------------------------------------------------
    // Awake triggers when this object first becomes active
    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindWithTag("Player").GetComponent<Actor>();
    }
    //what happens when this object is interacted with
    public override void OnInteract()
    {
        var sin = new EgoSin(boostFactor, duration, effectMaterial);
        player.myEffectHandler.AddTimedEffect(sin, duration);
        player.mySoundManager.PlaySound("CrowdCheer");
        player.StartCoroutine(FlexAnimationTimer());
        
        //if too much sin...
        Debug.Log($"applied: {EgoSin.applicationCount}, max: {EgoSin.sinMax}");
        if(EgoSin.applicationCount >= EgoSin.sinMax)
        {
            //update UI
            MenuManager.SIN_MENU.DisplaySin();
        }
        Cleanup();
    }

    //overload for OnInteract that takes an Actor, so anyone can interact with this object
    //Interactions done with way will not count as player interactions, and thus will not
    //increment the sin counter for this fight
    public void OnAnyInteract(Actor interactor)
    {
        var sin = new EgoSin(boostFactor, duration, effectMaterial, false);
        interactor.myEffectHandler.AddTimedEffect(sin, duration);
        Cleanup();
    }

    IEnumerator FlexAnimationTimer()
    {
        player.myAnimationHandler.TrySetBool("ego_flex", true);
        yield return new WaitForSeconds(flexTime);
        player.myAnimationHandler.TrySetBool("ego_flex", false);
    }

    //remove this instance of the crowd from the world
    void Cleanup()
    {
        Destroy(this.gameObject);
    }
}
