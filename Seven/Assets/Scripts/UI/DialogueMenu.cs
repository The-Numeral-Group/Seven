using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Yarn.Unity;

//In the dialogue canvas, make sure the screen space is set to camera, and add w/e camera to it.
[RequireComponent(typeof(RectTransform))]
public class DialogueMenu : BaseUI
{
    //Reference to yarnspinners dialogue runner. Expected to be set through Inspector
    [Tooltip("Reference to the dialogue runner script.")]
    public DialogueRunner dialogueRunner;
    //Set the speakerNameTextBox. Expected to be set through Inspector.
    [Tooltip("Reference to the canavas text element that holds the speakers name.")]
    public Text speakerNameTextBox;
    //reference to the chat bubble. Currently set to the inspector. Game will crash if null;
    [Tooltip("Reference to the canvas element which hold the chat bubble.")]
    public RectTransform chatBubble;
    //container for all the yarn dialogue files that need to be loaded into yarnspinner
    [Tooltip("Drag whatever yarn files the scene requires into this container")]
    public List<YarnProgram> dialogueFiles;
    //reference to the text passed in from yarnspinner.
    public string untrimmedText {get; set;}

    //reference to the ui menus canvas transform
    RectTransform canvasTransform;

    public delegate void TestDelegate();
    TestDelegate dialogueDelegateCallback;

    protected override void Awake()
    {
        base.Awake();
        foreach(YarnProgram yarnFile in dialogueFiles)
        {
            dialogueRunner.Add(yarnFile);
        }
    }

    //Initialize non inspector set fields.
    void Start()
    {
        canvasTransform = GetComponent<RectTransform>();
    }

    //Move the chat bubble.
    void FixedUpdate()
    {
        if (ActiveSpeaker.ACTIVE_NPC)
        {
            chatBubble.localPosition = GetWorldPosition();
        }
    }

    //Callback function used by the dialoguemenu ui object
    public void OnDialogueEndCallback()
    {
        ActiveSpeaker.ACTIVE_NPC.SetIsTalking(false);
        if (MenuManager.CURRENT_MENU == this)
        {
            MenuManager.CURRENT_MENU = null;
        }
        else
        {
            Debug.LogWarning("DialogueMenu: Dialogue is ending, but menu managers current menu" + 
            " is not pointing o the dialoguemenu.");
        }
        /*This line can cause an issue requiring the player to re-enter an npc's collision box
        to re-engage dialogue if the npc is in npcmode.*/
        if (!ActiveSpeaker.ACTIVE_NPC.npcMode)
        {
            ActiveSpeaker.ACTIVE_NPC = null;
            if (dialogueDelegateCallback != null)
            {
                dialogueDelegateCallback();
            }
            dialogueDelegateCallback = null;
        }
        ActiveSpeaker.ACTIVE_NPC = null;
        var player = GameObject.Find("/Player");
        if (!player)
        {
            Debug.LogWarning("DialogueMenu: OnDialogueFinish callback cannot find the player object.");
            return;
        }
        PlayerActor pActor = player.GetComponent<PlayerActor>();
        pActor.isTalking = false;
        pActor.playerInput.SwitchCurrentActionMap("Player");
        pActor.myHealth.enabled = true;
    }

    public void OnLineStartCallback()
    {
        //Credit for code: https://www.youtube.com/watch?v=gJrf6ON5UPE&t=333s
        string lineInfo = this.untrimmedText;
        if (lineInfo.Contains(":"))
        {
            string name = lineInfo.Substring(0, lineInfo.IndexOf(":"));
            if (ActiveSpeaker.ACTIVE_NPC.speakerName != name)
            {
                //insert to code to find the activespeaker who does share that name;
                var speakers = FindObjectsOfType<ActiveSpeaker>();
                bool found = false;
                foreach(var speaker in speakers)
                {
                    if (speaker.speakerName ==  name)
                    {
                        found = true;
                        ActiveSpeaker.ACTIVE_NPC.SetIsTalking(false);
                        ActiveSpeaker.ACTIVE_NPC = speaker;
                        ActiveSpeaker.ACTIVE_NPC.SetIsTalking(true);
                    }
                }
                if (!found)
                {
                    Debug.LogWarning("DialogueMenu: There was an parsing the actor names with the line: "
                    + lineInfo);
                }

            }
        }
    }

    /*Function to be utilized outside of class to start dialogue. Requires a gameobject to be passed in.
    The gameobject should reference a scene element which has n AcitveSpeaker component. the passed in method
    will be called once the dialogue finished
    IMPORTANT NOTE: THE ONLY TIME NO ARGUMENTS SHOULD BE PASSED IN IS WHEN THE PLAYER INITIATES
    DIALOGUE MANUALLY THROUGH PRESSING ATTACK NEAR AN NPC WHEN THE NPC IS IN NPCMODE.*/
    public void StartDialogue(GameObject npc = null, TestDelegate method = null, bool lockValue = true)
    {
        if (MenuManager.CanStartDialogue())
        {
            if (npc != null)
            {
                ActiveSpeaker newSpeaker = npc.GetComponent<ActiveSpeaker>();
                if (!newSpeaker)
                {
                    Debug.LogWarning("DialogueMenu: gameobject passed to StartDialogue() does not contain" + 
                        " an ActiveSpeaker component");
                    MenuManager.CURRENT_MENU = null;
                    return;
                }
                ActiveSpeaker.ACTIVE_NPC = newSpeaker;
                dialogueDelegateCallback = method;
            }
            else if (!ActiveSpeaker.ACTIVE_NPC)
            {
                Debug.LogWarning("DialogMenu: No active speaker has been set. If StartDialogue is " +
                "being called manually, ensure to pass in whatever gameobject you want as the speaker.");
                MenuManager.CURRENT_MENU = null;
                return;
            }
            ActiveSpeaker.ACTIVE_NPC.SetIsTalking(true);
            SetupPlayer(lockValue);
            dialogueRunner.StartDialogue(ActiveSpeaker.ACTIVE_NPC.yarnStartNode);
        }
    }

    /*function is used to setup the player to enter a dialogue sequence. lockValue is used
    to determine if the player can move during the dialogue sequence.*/
    public void SetupPlayer(bool lockValue)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (!player)
        {
            Debug.LogWarning("MenuManager: StartDialogue() cannot find the player object.");
            return;
        }
        PlayerActor pActor = player.GetComponent<PlayerActor>();
        if (pActor)
        {
            if (lockValue)
            {
                pActor.playerInput.SwitchCurrentActionMap("UI");
                pActor.myMovement.MoveActor(Vector2.zero);
            }
            pActor.isTalking = true;
            pActor.myHealth.enabled = false;
        }
    }

    //override baseui hide method.
    public override void Hide()
    {
        Debug.Log("DialogueMenu: Closing Dialogue menu.");
        dialogueRunner.dialogueUI.DialogueComplete();
        //OnDialogueEndCallback();
    }

    //Calculate the chatbubbles position on screen based on the active speakers location.
    Vector2 GetWorldPosition()
    {
        Vector2 viewPortPosition = 
            Camera.main.WorldToViewportPoint(ActiveSpeaker.ACTIVE_NPC.gameObject.transform.position 
            + new Vector3(0f, ActiveSpeaker.ACTIVE_NPC.spriteInfo.size.y / 2, 0f));
        Vector2 proportionalPosition = new Vector2(
            viewPortPosition.x * canvasTransform.sizeDelta.x,
            (viewPortPosition.y * canvasTransform.sizeDelta.y) + chatBubble.rect.height / 2);
        return proportionalPosition;
    }
}
