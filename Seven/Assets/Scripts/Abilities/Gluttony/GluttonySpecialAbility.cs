using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GluttonySpecialAbility : ActorAbilityFunction<Actor, int>
{
    //The sprite to be spawned in to show the attack. We could replace this with vfx.
    [Tooltip("The prefab that will be used to represent the visual part of the ability.")]
    public GameObject toInstantiateAbilitySprite;
    //How far away to spawn the specialAbilitySprite relative to the owner of the ability
    [Tooltip("How far away to spawn the prefab relative to the user.")]
    public Vector2 distanceFromActor = new Vector2(0f, -6f);

    //The duration of the special ability
    [Tooltip("How long the ability will last for.")]
    public float duration = 5f;

    //if at 0 the targetActor will not get dragged.
    [Tooltip("The speed at which the target is dragged. if 0 no drag")]
    public float specialSpeed = 1f;
    //The actor this ability will target. Specifically their movement component 
    Actor targetActor;

    //Initialize monobehavior fields
    void Start()
    {
        var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];
        if(playerObject == null)
        {
            Debug.LogWarning("GluttonySpecialP1: Gluttony can't find the player!");
        }
        else
        {
            targetActor = playerObject.GetComponent<Actor>();
        }
    }

    //Similar to ActortAbilityFunction Invoke. additional checks for valid target and isFinished
    public override void Invoke(ref Actor user)
    {
        //If we have not referenced a target this ability will not initiate.
        if(this.usable && targetActor && this.isFinished)
        {
            this.isFinished = false;
            StartCoroutine(coolDown(cooldownPeriod));
            InternInvoke(user);
        }
    }

    /*Internalinvoke will spawn a sprite to visualize the move. 
    The target actor is then dragged towards that stop for the specified duration.*/
    protected override int InternInvoke(params Actor[] args)
    {
        Vector3 spawnPos = new Vector3(args[0].gameObject.transform.position.x + distanceFromActor.x,
                                       args[0].gameObject.transform.position.y + distanceFromActor.y,
                                       args[0].gameObject.transform.position.z); 
        GameObject specialAbilitySprite = Instantiate(toInstantiateAbilitySprite, spawnPos, Quaternion.identity);
        IEnumerator dragTargetActor = DragTargetActor(spawnPos);
        IEnumerator stopSpecialAbility = StopSpecialAbility(dragTargetActor, specialAbilitySprite);
        StartCoroutine(dragTargetActor); //Thse two must be done hand in hand. The second coroutine is responsible for killing the first
        StartCoroutine(stopSpecialAbility);
        return 0;
    }
    
    //Drags the target actor in the direction of the destination vector
    IEnumerator DragTargetActor(Vector3 spawnPos)
    {
        while (true && targetActor)
        {
            yield return new WaitForFixedUpdate();
            //I (Ram) am not sure if I need to normalize the drag direction vector.
            Vector2 destination = (spawnPos - targetActor.gameObject.transform.position).normalized;
            destination = destination * specialSpeed;
            targetActor.myMovement.DragActor(destination);
        }
    }

    //Responsible for stopping the coroutine which drags the player.
    IEnumerator StopSpecialAbility(IEnumerator toStop, GameObject toDestroy)
    {
        yield return new WaitForSeconds(duration);
        StopCoroutine(toStop);
        Destroy(toDestroy);
        this.isFinished = true;
    }

    //Kill the player if they get sucked up by gluttony when this move is active.
    void OnCollisionEnter2D(Collision2D collider)
    {
        if (!this.isFinished && collider.gameObject == targetActor.gameObject)
        {
             var enemyHealth = collider.gameObject.GetComponent<ActorHealth>();

            //or a weakpoint if there's no regular health
            if(enemyHealth == null){collider.gameObject.GetComponent<ActorWeakPoint>();}

            //if the enemy can take damage (if it has an ActorHealth component),
            //hurt them. Do nothing if they can't take damage.
            if(enemyHealth != null){
                if (!enemyHealth.vulnerable)
                {
                    return;
                }
                enemyHealth.takeDamage(enemyHealth.currentHealth);
            }
        }
    }
}
