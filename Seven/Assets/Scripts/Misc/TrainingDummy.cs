using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ActorHealth))]
public class TrainingDummy : MonoBehaviour
{
    ActorHealth health;

    void Start()
    {
        health = this.gameObject.GetComponent<ActorHealth>();
    }
    public void DoActorDamageEffect(float damage)
    {
        Debug.Log($"TrainingDummy: took {damage} damage");
    }

    public void DoActorDeath()
    {
        Debug.Log("TrainingDummy: is slain!");
        health.currentHealth = health.startingMaxHealth;
    }
}
