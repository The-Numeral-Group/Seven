using UnityEngine;
using UnityEngine.UI;

//Document Link: https://docs.google.com/document/d/1FJHepZKLViAMvSNGTv_D2U5VYQ5q0EMsbegKWNF93oo/edit?usp=sharing
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
    BattleUI battleUI = null;
    //Static reference ot the Battle UI
    public static BattleUI BATTLE_UI;
    //Reference to the interaction menu
    [SerializeField]
    [Tooltip("Refereence to InteractMenu ui object. Must be set via inspector.")]
    InteractMenu interactMenu = null;
    //static reference to the interact menu
    public static InteractMenu INTERACT_MENU;
    /*Reference to the Game Over UI. Must be set through inspector because currently
    gameover object is a child of the the menumanager object.*/
    [Tooltip("Reference to the gameover ui child object. Must be set via inspector.")]
    [SerializeField]
    GameOver gameOver = null;
    //Static reference to game over
    public static GameOver GAME_OVER;
    //pointer to the current opened menu.
    public static BaseUI CURRENT_MENU;
    //Gameover flag
    public static bool GAME_IS_OVER = false;

    //Set static members to the inspector references
    void Awake()
    {
        CURRENT_MENU = null;
        SetReferences<DialogueMenu>(ref dialogueMenu, ref DIALOGUE_MENU, "DialogueMenu");
        SetReferences<PauseMenu>(ref pauseMenu, ref PAUSE_MENU, "PauseMenu");
        SetReferences<BattleUI>(ref battleUI, ref BATTLE_UI, "BattleUI");
        GAME_OVER = gameOver;
        INTERACT_MENU = interactMenu;
        GAME_OVER.Hide();
        INTERACT_MENU.Hide();
        if (PAUSE_MENU)
        {
            PAUSE_MENU.Hide();
        }
    }

    /*Starts the dialogue menu via yarnspinner. Will crash if activenpc has not already been set.
    Utilized DialogueMenu*/
    public static bool CanStartDialogue()
    {
        if(!DIALOGUE_MENU)
        {
            Debug.LogWarning("MenuManager: DialogMenu not hooked up properly.");
            return false;
        }
        if (CURRENT_MENU)
        {
            Debug.LogWarning("MenuManager: There is already a menu active - " + CURRENT_MENU.GetType());
            return false;
        }
        CURRENT_MENU = DIALOGUE_MENU;
        return true;
    }

    /*Starts the pause menu. Used as the callback from the playerinputs OnMenu function.
    returns false if the pause menu is not started*/
    public static bool StartPauseMenu()
    {
        if (!CURRENT_MENU && PAUSE_MENU)
        {
            CURRENT_MENU = PAUSE_MENU;
            PAUSE_MENU.Show();
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

    //Function to start the game over sequence. Hides menus and opens the game over ui.
    public static void StartGameOver()
    {
        GAME_IS_OVER = true;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerActor>().playerInput.SwitchCurrentActionMap("UI");
        if (CURRENT_MENU)
        {
            CURRENT_MENU.Hide();
        }
        if (BATTLE_UI)
        {
            BATTLE_UI.Hide();
        }
        CURRENT_MENU = GAME_OVER;
        Time.timeScale = 0f;
        GAME_OVER.Show();
    }

}
