using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SlothMeleeMarker : MonoBehaviour
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("How much damage this marker should cause.")]
    public float damage = 2f;

    [Tooltip("How long this marker should stay on the floor.")]
    public float duration = 1.5f;

    //an activity bool to disable damage in certain circumstances
    private bool active = true;

    //METHODS--------------------------------------------------------------------------------------
    void Start()
    {
        //wanted to make this an anonymous method, couldn't figure it out lol
        StartCoroutine(AutoDestroy(duration));
    }

    //called when something leaves this trigger volume
    void OnTriggerExit2D(Collider2D collided)
    {
        var health = collided.gameObject.GetComponent<ActorHealth>();

        if(health && active)
        {
            health.takeDamage(damage);
            CleanupMarker();
        }
    }

    //destroys the game object after a duration
    IEnumerator AutoDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        CleanupMarker();
    }

    //performs needed cleanup for destroying this marker
    void CleanupMarker()
    {
        active = false;
        Destroy(this.gameObject);
    }
}
