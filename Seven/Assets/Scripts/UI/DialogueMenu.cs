using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueMenu : BaseUI
{
    //Reference to yarnspinners dialogue runner. Expected to be set through Inspector
    public DialogueRunner dialogueRunner;
    //Set the speakerNameTextBox. Expected to be set through Inspector.
    public Text speakerNameTextBox;
    //reference to the chat bubble;
    public RectTransform chatBubble;

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
    }

    //override baseui hide method.
    public override void Hide()
    {
        Debug.Log("DialogueMenu: Closing Dialogue menu.");
        MenuManager.CURRENT_MENU = null;
        dialogueRunner.dialogueUI.DialogueComplete();
        //OnDialogueEndCallback();
    }
}
