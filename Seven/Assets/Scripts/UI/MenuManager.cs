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
    RectTransform dialogueCanvasTransform;
    //static reference to the Canvas transform
    public static RectTransform DIALOGUE_CANVAS_TRANSFORM;
    //reference to the Pause Menu. Expected to be set via Inspector.
    [Tooltip("Refernce to the Pause Menu gameobject in scene.")]
    [SerializeField]
    PauseMenu pauseMenu = null;
    //Static reference to the pause menu
    public static PauseMenu PAUSE_MENU;

    //pointer to the current opened menu.
    public static BaseUI CURRENT_MENU;

    //Set static members to the inspector references
    void Awake()
    {
        SetReferences<DialogueMenu>(ref dialogueMenu, ref DIALOGUE_MENU, "/DialogueMenu");
        SetReferences<PauseMenu>(ref pauseMenu, ref PAUSE_MENU, "/PauseMenu");
        PAUSE_MENU.gameObject.SetActive(false);
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

    /*Starts the dialogue menu via yarnspinner. Will crash if activenpc has not already been set.
    Utilized by the players StartTalking function.*/
    public static void StartDialogue()
    {
        if(!DIALOGUE_MENU)
        {
            Debug.LogWarning("MenuManager: DialogMenu not hooked up properly.");
            return;
        }
        CURRENT_MENU = DIALOGUE_MENU;
        DIALOGUE_MENU.dialogueRunner.StartDialogue(ActiveSpeaker.ACTIVE_NPC.yarnStartNode);
    }

    //Starts the pause menu. Used as the callback from the playerinputs OnMenu function.
    public static void StartPauseMenu()
    {
        if (!CURRENT_MENU && PAUSE_MENU)
        {
            CURRENT_MENU = PAUSE_MENU;
            PAUSE_MENU.gameObject.SetActive(true);
            PAUSE_MENU.PauseGame();
        }
        else
        {
            CURRENT_MENU.Hide();
        }
    }

    //Helper function to set references and static references for ui options.
    void SetReferences<T>(ref T inspectorRef, ref T STATIC_REF, string objectName)
    {
        if(inspectorRef == null)
        {
            var fooMenu = GameObject.Find(objectName);
            if (fooMenu) 
            {
                inspectorRef = fooMenu.GetComponent<T>();
            }
        }
        STATIC_REF = inspectorRef;
        if (STATIC_REF == null)
        {
            Debug.LogWarning("MenuManager: " + objectName + " not hooked up properly.");
        }
    }
}
