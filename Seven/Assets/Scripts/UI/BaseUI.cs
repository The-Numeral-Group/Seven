using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Base abstract class for ui related classes.
[RequireComponent(typeof(Canvas))]
public abstract class BaseUI : MonoBehaviour
{
    protected Canvas canvas;
    protected virtual void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }
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
        if (MenuManager.CURRENT_MENU == this)
        {
            MenuManager.CURRENT_MENU = null;
        }
        this.gameObject.SetActive(false);
    }

    public virtual void LoadScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public virtual void CloseApplication()
    {
        Application.Quit();
    }
}
