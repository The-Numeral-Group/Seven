using UnityEngine;
using UnityEngine.UI;

//Class used for swapping ui images on input changes
public class SwapUIImage : MonoBehaviour
{
    //Reference to the image this ui is utilizing
    public Image image;
    [Tooltip("sprite to be loaded into image on keyboard swap.")]
    public Sprite keyboardSprite;
    [Tooltip("sprite to be loaded into image on controller swap.")]
    public Sprite gamepadSprite;

    public void SwapImage(string currentControlScheme)
    {
        if(currentControlScheme == "Gamepad")
        {
            image.sprite = gamepadSprite;
        }
        else
        {
            image.sprite = keyboardSprite;
        }
    }
   
}
