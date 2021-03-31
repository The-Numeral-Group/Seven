using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Document Link: https://docs.google.com/document/d/11VlBnrazcBwxiyJNak_8vkbsGFwRt6HZuifmm9eFGFk/edit?usp=sharing
[RequireComponent(typeof(Collider2D))]
public class GluttonySin : MonoBehaviour, ActorEffect
{
    //static value so every GluttonySin knows how many have
    //already been activated
    public static int maxGluts = 3;
    public static int appliedGluts = 0;

    //Strength of Gluttony's sin
    [Tooltip("Percentage of the player's original max health that will be ADDED (1 is 100%)")]
    public float healthIncrease = 1.0f;
    [Tooltip("Percentage of the player's original speed that will be REMOVED (1 is 100%)")]
    public float speedDecrease = 0.3f;
    [Tooltip("Should this be deleted if the related effect stops?")]
    public bool deleteOnEffectEnd = true;

    //when this thing gets walked into...
    void OnTriggerEnter2D(Collider2D collider){
        //The sin should only be triggered on player collision
        if (collider.gameObject.tag != "Player")
        {
            return;
        }
        Debug.Log("collision with food!");

        //aquire the ActorEffectHandler of whoever ran into the food
        var collidedEffectHandler = collider.gameObject.GetComponent<ActorEffectHandler>();

        //if the thing we collided with has an ActorEffectHandler component,
        //apply ourselves to it
        if(collidedEffectHandler != null){
            collidedEffectHandler.AddEffect(this);
        }
    }

    
    public bool ApplyEffect(ref Actor actor){
        if(GluttonySin.appliedGluts < GluttonySin.maxGluts){
            var health = actor.myHealth;
            var movement = actor.myMovement;

            health.maxHealth += health.startingMaxHealth * this.healthIncrease;
            health.currentHealth += health.startingMaxHealth * this.healthIncrease;

            movement.speed -= movement.speed * this.speedDecrease;

            ++GluttonySin.appliedGluts;

            this.gameObject.SetActive(false);

            Debug.Log("Food eaten!");
            return true;
        }
        Debug.Log("Too much glut!");
        return false;
    }

    public void RemoveEffect(ref Actor actor){
        if(GluttonySin.appliedGluts > 0){
            var health = actor.myHealth;
            var movement = actor.myMovement;

            health.maxHealth -= health.startingMaxHealth * this.healthIncrease;
            health.currentHealth -= health.startingMaxHealth * this.healthIncrease;

            movement.speed += movement.speed * this.speedDecrease;

            --GluttonySin.appliedGluts;
        }

        if(this.deleteOnEffectEnd){
            Destroy(this.gameObject);
        }
    }
}
