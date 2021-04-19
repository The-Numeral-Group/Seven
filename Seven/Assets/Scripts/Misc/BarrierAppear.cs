using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierAppear : MonoBehaviour
{
    private SpriteRenderer sr;

    public float endOpacity;
    public float duration;
    
    // Start is called before the first frame update
    void Start()
    {
        this.sr = this.GetComponent<SpriteRenderer>();
        StartCoroutine(appearAnimation());
    }

    private IEnumerator appearAnimation()
    {
        float opacity = 0f;
        while (opacity < endOpacity)
        {
            opacity += 0.1f;
            sr.color = new Color(255f, 255f, 255f, opacity);
            yield return new WaitForSeconds(this.duration / 10);
        }
    }
}
