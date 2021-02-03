using UnityEngine;
using UnityEngine.UI;

/*This class serves as a container for the the various menus present in the game.
I (Ram) created this as a means to access various menu elements from other classes globally.*/
public class MenuManager : MonoBehaviour
{
    //Reference to the dialoguemenu. Reference expected to be set through inspector
    [SerializeField]
    [Tooltip("Reference to the dialoguemenu object.")]
    DialogueMenu dialogueMenu;
    //static reference to the Dialogue menu;
    public static DialogueMenu DIALOGUE_MENU;

    //Set static members to the inspector references
    void Start()
    {
        DIALOGUE_MENU = dialogueMenu;
        if (!DIALOGUE_MENU)
        {
            Debug.LogWarning("MenuManager: DialogMenu not hooked up properly.");
        }
    }
}
