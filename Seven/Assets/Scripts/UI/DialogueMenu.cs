using UnityEngine;
using UnityEngine.UI;
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
        MenuManager.CURRENT_MENU = null;
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
