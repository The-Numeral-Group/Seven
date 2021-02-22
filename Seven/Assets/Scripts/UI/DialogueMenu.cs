using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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

    //reference to the ui menus canvas transform
    RectTransform canvasTransform;

    public delegate void TestDelegate();
    TestDelegate dialogueDelegateCallback;

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
        var player = GameObject.Find("/Player");
        ActiveSpeaker.ACTIVE_NPC.SetIsTalking(false);

        if (!player)
        {
            Debug.LogWarning("DialogueMenu: OnDialogueFinish callback cannot find the player object.");
            return;
        }
        PlayerActor pActor = player.GetComponent<PlayerActor>();
        pActor.isTalking = false;
        pActor.playerInput.SwitchCurrentActionMap("Player");
        pActor.myHealth.enabled = true;
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
    }

    /*Function to be utilized outside of class to start dialogue. Requires a gameobject to be passed in.
    The gameobject should reference a scene element which has n AcitveSpeaker component. the passed in method
    will be called once the dialogue finished*/
    public void StartDialogue(GameObject npc, TestDelegate method = null)
    {
        ActiveSpeaker newSpeaker = npc.GetComponent<ActiveSpeaker>();
        if (!newSpeaker)
        {
            Debug.LogWarning("DialogueMenu: gameobject passed to StartDialogue() does not contain" + 
                " an ActiveSpeaker component");
            return;
        }
        ActiveSpeaker.ACTIVE_NPC = newSpeaker;
        dialogueDelegateCallback = method;
        MenuManager.StartDialogue();
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
