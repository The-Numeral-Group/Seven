using UnityEngine;
using UnityEngine.UI;

/*Class is meant to be attached to ui objects that serve as sub menus within a larger menu ui object.
Primary use is to serve as a container for values*/
public class SubMenu : MonoBehaviour
{
    [Tooltip("The name of this sub menu")]
    public string menuName;
    [Tooltip("The defauly button that should be selected when this menu is active.")]
    public Button defaultButton;

    public virtual void Show()
    {
        this.gameObject.SetActive(true);
        defaultButton.Select();
    }
}
