using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathFBShadow : MonoBehaviour
{ 
    // Maximum scale the shadow will get to
    public float maxScale;
    
    // Delay before shadow gets bigger to indicate that flaming ball is dropping
    public float delayShadow;

    // Delay for iteration of shadow getting bigger
    public float delayShadowScale;

    // Delay after the fireball drops
    public float delayFire;

    // Delay to turn off shadow sprite renderer
    private float delayShadowOff = 0.3f;

    private float delaySpeedMultiplier;


    // Start is called before the first frame update
    void Start()
    {
        delaySpeedMultiplier = WrathP2Actor.abilitySpeedMultiplier;
        StartCoroutine(StartShadow());
    }

    private IEnumerator StartShadow()
    {
        // Delay before shadow gets bigger
        yield return new WaitForSeconds(delayShadow / delaySpeedMultiplier);

        this.gameObject.GetComponent<ActorSoundManager>().PlaySound("firebrim");

        // Play animation to make shadows bigger
        while (this.gameObject.transform.localScale.x < maxScale){
            this.gameObject.transform.localScale += new Vector3(0.1f, 0.1f, 0f);
            yield return new WaitForSeconds(delayShadowScale / delaySpeedMultiplier);

        }

       // this.gameObject.GetComponent<ActorSoundManager>().PlaySound("firebrim");

        // Now Fireball drops.
        this.transform.GetChild(0).gameObject.SetActive(true);

        // Delay before turning off shadow Sprite Renderer
        yield return new WaitForSeconds(delayShadowOff / delaySpeedMultiplier);
        this.GetComponent<SpriteRenderer>().enabled = false;

        // Delay before the object gets destroyed
        yield return new WaitForSeconds(delayFire / delaySpeedMultiplier);

        Destroy(this.gameObject);
    }
}
