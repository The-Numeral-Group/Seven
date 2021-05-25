using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathFBShadow : MonoBehaviour
{
    // Maximum scale the shadow will get to
    public float maxScale;
    
    // Delay before shadow gets bigger to indicate that flaming ball is dropping
    public float delayShadow;

    // Delay after the fireball drops
    public float delayFire;

    // Delay to turn off shadow sprite renderer
    private float delayShadowOff = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartShadow());
    }

    private IEnumerator StartShadow()
    {
        // Delay before shadow gets bigger
        yield return new WaitForSeconds(delayShadow);

        // Play animation to make shadows bigger
        while(this.gameObject.transform.localScale.x < maxScale){
            this.gameObject.transform.localScale += new Vector3(0.1f, 0.1f, 0f);
            yield return new WaitForFixedUpdate();

        }

        // Now Fireball drops.
        this.transform.GetChild(0).gameObject.SetActive(true);

        // Delay before turning off shadow Sprite Renderer
        yield return new WaitForSeconds(delayShadowOff);
        this.GetComponent<SpriteRenderer>().enabled = false;

        // Delay before the object gets destroyed
        yield return new WaitForSeconds(delayFire);

        Destroy(this.gameObject);
    }
}
