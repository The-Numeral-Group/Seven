using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonImageChange : MonoBehaviour
{   [Tooltip("The image to display on click with no save file")]
    public Sprite failImage;

    [Tooltip("The GameSaveManager to check for save files in")]
    public GameObject GameSaveManager;

    [Tooltip("The length of time the image should last")]
    public float switch_duration;

    public Vector2 newImageSize;

    private Vector2 originalImageSize;

    //the object's Image renderer
    private Image render;

    //the original Image of the object
    private Sprite origImage;

    // Start is called before the first frame update
    void Start()
    {
        render = this.gameObject.GetComponent<Image>();
        originalImageSize = this.gameObject.GetComponent<RectTransform>().sizeDelta;
        origImage = render.sprite;
    }

    public void ImageChangePerm(Sprite newImage)
    {
        render.sprite = newImage;
        origImage = newImage;
    }

    public void ImageChangeTemp(Sprite newImage)
    {
        StartCoroutine(changeImageDuration(newImage));
    }

    private IEnumerator changeImageDuration(Sprite newImage)
    {
        render.sprite = newImage;
        this.gameObject.GetComponent<RectTransform>().sizeDelta = newImageSize;
        yield return new WaitForSeconds(switch_duration);
        this.gameObject.GetComponent<RectTransform>().sizeDelta = originalImageSize;
        render.sprite = origImage;
    }

    public void checkSaveFileExists(Sprite newImage = null)
    {
        if (!File.Exists(Application.persistentDataPath + ("/SaveFile.dat")))
        {
            this.ImageChangeTemp(failImage);
        }
        else if (newImage != null)
        {
            this.ImageChangeTemp(newImage);
        }
    }

    public void checkSaveFileExistsNoSuccessImage()
    {
        if (!File.Exists(Application.persistentDataPath + ("/SaveFile.dat")))
        {
            this.ImageChangeTemp(failImage);
        }
    }
}
