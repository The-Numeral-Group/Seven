using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

//Document link: https://docs.google.com/document/d/18lYFFhUkIy72RahBtPS0Hf00NOF2xylJcnMHjfXqQbE/edit?usp=sharing
//Base abstract class for ui related classes.
[RequireComponent(typeof(Canvas))]
public abstract class BaseUI : MonoBehaviour
{
    protected Canvas canvas;
    [Tooltip("Reference to the selector game boject indicator. Do not set a reference if the ui does not need a selector image.")]
    public RectTransform uiSelector;
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

    protected virtual void Update()
    {
        if (uiSelector != null)
        {
            uiSelector.transform.position = EventSystem.current.currentSelectedGameObject.transform.position + 
                new Vector3(EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().sizeDelta.x,0,0);
        }
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
