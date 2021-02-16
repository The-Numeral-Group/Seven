using UnityEngine;
using UnityEngine.UI;

//Base abstract class for ui related classes.
public abstract class BaseUI : MonoBehaviour
{
    /*Set the whether the menu is active/visiable*/
    public virtual void SetVisibility(bool value)
    {
        this.gameObject.SetActive(value);
    }

    /*enable the menu*/
    public virtual void Show()
    {
        this.gameObject.SetActive(true);
    }

    /*disable the menu*/
    public virtual void Hide()
    {
        MenuManager.CURRENT_MENU = null;
        this.gameObject.SetActive(false);
    }
}
