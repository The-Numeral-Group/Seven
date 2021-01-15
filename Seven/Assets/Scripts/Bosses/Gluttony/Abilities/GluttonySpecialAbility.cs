﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GluttonySpecialAbility : ActorAbilityFunction<Actor, int>
{
    //The sprite to be spawned in to show the attack. We could replace this with vfx.
    public GameObject toInstantiateAbilitySprite;
    //The actor this ability will target. Specifically their movement component 
    public Actor targetActor;
    //How far away to spawn the specialAbilitySprite relative to the owner of the ability
    public Vector2 distanceFromActor = new Vector2(0f, -6f);

    //Ideally duration and cooldown are the same.
    public float duration = 5f;

    //if at 0 the targetActor will not get dragged.
    public float specialSpeed = 1f;

    private void Start()
    {
        //Start is there in order to make sure an actor has been selected as the target.
        //There is definitely a cleaner way of doing this that I (Ram) have not bothered thinking of.
        if (this.targetActor == null)
        {
            Debug.Log("GluttonySpecialAbility: No valid Actor target provided.");
        }
    }

    public override void Invoke(ref Actor user)
    {
        //If we have not referenced a target this ability will not initiate.
        if(usable && this.targetActor)
        {
            InternInvoke(user);
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    //The ability will spawn a sprite to visual the move. The target actor is then dragged towards that stop for the specified duration.
    protected override int InternInvoke(Actor[] args)
    {
        Vector3 spawnPos = new Vector3(args[0].gameObject.transform.position.x + this.distanceFromActor.x,
                                       args[0].gameObject.transform.position.y + this.distanceFromActor.y,
                                       args[0].gameObject.transform.position.z); 
        GameObject specialAbilitySprite = Instantiate(this.toInstantiateAbilitySprite, spawnPos, Quaternion.identity);

        //I (Ram) Do not have a full understanding of how invoke repeating works.
        //0.02 is 1/50th of a second. I Believe fixedupdate runs at 50fps. Therefore Invoke Repeating is called 50 times per seconds in sync with fixedupdate.
        //InvokeRepeating("DragTargetActor", 0, 0.02f);
        IEnumerator dragTargetActor = DragTargetActor(spawnPos);
        IEnumerator stopSpecialAbility = StopSpecialAbility(dragTargetActor, specialAbilitySprite);
        StartCoroutine(dragTargetActor); //Thse two must be done hand in hand. The second coroutine is responsible for killing the first
        StartCoroutine(stopSpecialAbility);
        return 0;
    }
    
    //Drags the target actor in the direction of the destination vector
    private IEnumerator DragTargetActor(Vector3 spawnPos)
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            //I (Ram) am not sure if I need to normalize the drag direction vector.
            Vector2 destination = (spawnPos - this.targetActor.gameObject.transform.position).normalized;
            destination = destination * this.specialSpeed;
            targetActor.myMovement.DragActor(destination);
        }
    }

    //Responsible for stopping the coroutine which drags the player.
    private IEnumerator StopSpecialAbility(IEnumerator toStop, GameObject toDestroy)
    {
        yield return new WaitForSeconds(this.duration);
        StopCoroutine(toStop);
        Destroy(toDestroy);
    }
}
