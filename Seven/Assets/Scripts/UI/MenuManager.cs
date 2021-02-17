using UnityEngine;
using UnityEngine.UI;

/*This class serves as a container for the the various menus present in the game.
I (Ram) created this as a means to access various menu elements from other classes globally.*/
public class MenuManager : MonoBehaviour
{
    //Reference to the dialoguemenu. Reference can be set through inspector
    [SerializeField]
    [Tooltip("Reference to the dialoguemenu object in scene.")]
    DialogueMenu dialogueMenu = null;
    //static reference to the Dialogue menu;
    public static DialogueMenu DIALOGUE_MENU;
    //reference to the Pause Menu. reference can be set via Inspector.
    [Tooltip("Refernce to the Pause Menu gameobject in scene.")]
    [SerializeField]
    PauseMenu pauseMenu = null;
    //Static reference to the pause menu
    public static PauseMenu PAUSE_MENU;
    //Reference to the BattleUI. 
    [Tooltip("Reference to the battleui object in scene.")]
    [SerializeField]
    BattleUI battleUI;
    //Static reference ot the Battle UI
    public static BattleUI BATTLE_UI;
    //Reference to the Game Over UI. Must be set through inspector 
    [Tooltip("Reference to the gameover ui child object. Must be set via inspector.")]
    [SerializeField]
    GameOver gameOver = null;
    //Static reference to game over
    public static GameOver GAME_OVER;

    //pointer to the current opened menu.
    public static BaseUI CURRENT_MENU;
    public static bool GAME_IS_OVER = false;

    //Set static members to the inspector references
    void Awake()
    {
        SetReferences<DialogueMenu>(ref dialogueMenu, ref DIALOGUE_MENU, "DialogueMenu");
        SetReferences<PauseMenu>(ref pauseMenu, ref PAUSE_MENU, "PauseMenu");
        SetReferences<BattleUI>(ref battleUI, ref BATTLE_UI, "BattleUI");
        GAME_OVER = gameOver;
        GAME_OVER.gameObject.SetActive(false);
        if (PAUSE_MENU)
        {
            PAUSE_MENU.Hide();
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

    /*Starts the pause menu. Used as the callback from the playerinputs OnMenu function.
    returns false if the pause menu is not started*/
    public static bool StartPauseMenu()
    {
        if (!CURRENT_MENU && PAUSE_MENU)
        {
            CURRENT_MENU = PAUSE_MENU;
            PAUSE_MENU.Show();
            PAUSE_MENU.PauseGame();
            return true;
        }
        else
        {
            if (CURRENT_MENU)
            {
                CURRENT_MENU.Hide();
            }
            else
            {
                Debug.LogWarning("MenuManager: Attempted to close a menu that" + 
                    "is being referenced by CURRENT_MENU.");
            }
            return false;
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

    public static void StartGameOver()
    {
        GAME_IS_OVER = true;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerActor>().playerInput.SwitchCurrentActionMap("UI");
        if (PAUSE_MENU)
        {
            PAUSE_MENU.Hide();
        }
        if (BATTLE_UI)
        {
            BATTLE_UI.Hide();
        }
        Time.timeScale = 0f;
        GAME_OVER.Show();
    }

}
