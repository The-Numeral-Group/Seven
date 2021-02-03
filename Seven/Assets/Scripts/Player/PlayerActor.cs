using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerActor : Actor
{
    //A flag to notify if the player is talking with someone.
    public bool isTalking {get; set;}
    //Reference to the player input component
    PlayerInput myInput;

    //Temporary reference
    public DialogueMenu dm;
    protected override void Start()
    {
        base.Start();
        //Setting to true for testing
        myInput = GetComponent<PlayerInput>();
    }

    public void OnDialogueEnd()
    {
        isTalking = false;
        myInput.SwitchCurrentActionMap("Player");
        this.myHealth.enabled = true;
        this.myHealth.vulnerable = true;
    }

    public void StartTalking()
    {
        isTalking = true;
        myInput.SwitchCurrentActionMap("UI");
        /*POTENTIAL BUG NOTE (Ram): If the player dodges just prior to talking it could
        end up making the player vulnerable while they are talking.*/
        this.myHealth.vulnerable = false;
        this.myHealth.enabled = false;
        dm.dialogueRunner.StartDialogue(ActiveSpeaker.ACTIVE_NPC.yarnStartNode);
    }
}
