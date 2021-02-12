using UnityEngine;
using UnityEngine.UI;

/*This class serves as a container for the the various menus present in the game.
I (Ram) created this as a means to access various menu elements from other classes globally.*/
public class MenuManager : MonoBehaviour
{
    //Reference to the dialoguemenu. Reference expected to be set through inspector
    [SerializeField]
    [Tooltip("Reference to the dialoguemenu object.")]
    DialogueMenu dialogueMenu = null;
    //static reference to the Dialogue menu;
    public static DialogueMenu DIALOGUE_MENU;
    //Reference to the canvas rectTransform
    [SerializeField]
    [Tooltip("Reference to the canvas rectangular transform.")]
    RectTransform dialogueCanvasTransform = null;
    //static reference to the Canvas transform
    public static RectTransform DIALOGUE_CANVAS_TRANSFORM;

    //Set static members to the inspector references
    void Awake()
    {
        //Setup dialogue menu references
        if(!dialogueMenu)
        {
            var dMenu = GameObject.Find("/DialogueMenu");
            if (dMenu) 
            {
                dialogueMenu = dMenu.GetComponent<DialogueMenu>();
            }
        }
        DIALOGUE_MENU = dialogueMenu;
        if (!DIALOGUE_MENU)
        {
            Debug.LogWarning("MenuManager: DialogMenu not hooked up properly.");
        } 
        else 
        {
            if (!dialogueCanvasTransform)
            {
                dialogueCanvasTransform = dialogueMenu.gameObject.GetComponent<RectTransform>();
            }
            DIALOGUE_CANVAS_TRANSFORM = dialogueCanvasTransform;
            if (!DIALOGUE_CANVAS_TRANSFORM)
            {
                Debug.LogWarning("MenuManager: CanvasTransform not hooked up properly.");
            }
        }
    }

    public static void StartDialogue()
    {
        if(!DIALOGUE_MENU)
        {
            Debug.LogWarning("MenuManager: DialogMenu not hooked up properly.");
        }
        DIALOGUE_MENU.dialogueRunner.StartDialogue(ActiveSpeaker.ACTIVE_NPC.yarnStartNode);
    }
}
