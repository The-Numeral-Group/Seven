using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeBlackScreenEffect : MonoBehaviour
{
    public Image image;
    public float fadeIn_duration;
    public float fadeOut_duration;

    // Start is called before the first frame update
    void Start()
    {
        image = this.gameObject.GetComponent<Image>();
    }

    public void startFadeIn()
    {
        StartCoroutine(fadeIn());
    }

    private IEnumerator fadeIn()
    {
        float opacity = 0f;
        while (opacity < 1f)
        {
            opacity += 0.1f;
            image.color = new Color(image.color.r, image.color.g, image.color.b, opacity);
            yield return new WaitForSeconds(this.fadeIn_duration / 10);
        }
    }

    public void startFadeOut()
    {
        StartCoroutine(fadeOut());
    }

    private IEnumerator fadeOut()
    {
        float opacity = 1f;
        while (opacity > 0f)
        {
            opacity -= 0.1f;
            image.color = new Color(image.color.r, image.color.g, image.color.b, opacity);
            yield return new WaitForSeconds(this.fadeOut_duration / 10);
        }
    }
}
