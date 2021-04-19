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
    }
}
