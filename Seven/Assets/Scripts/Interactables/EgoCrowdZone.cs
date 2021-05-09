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
        var sin = new EgoSin(boostFactor, duration);
        player.myEffectHandler.AddTimedEffect(sin, duration);
        Cleanup();
    }

    //overload for OnInteract that takes an Actor, so anyone can interact with this object
    //Interactions done with way will not count as player interactions, and thus will not
    //increment the sin counter for this fight
    public void OnInteract(Actor interactor)
    {
        var sin = new EgoSin(boostFactor, duration, false);
        interactor.myEffectHandler.AddTimedEffect(sin, duration);
        Cleanup();
    }

    //remove this instance of the crowd from the world
    void Cleanup()
    {
        Destroy(this.gameObject);
    }
}
