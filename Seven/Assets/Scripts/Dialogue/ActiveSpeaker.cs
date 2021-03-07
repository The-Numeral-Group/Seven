using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Activespeaker is a class you can attach to any actor you want to be able to talk to the player.
DO NOT ATTACH THIS OBJECT TO THE PLAYER*/
//Credit for this concept: https://www.youtube.com/watch?v=CJu0ObGDQHY&t=317s
[RequireComponent(typeof(SpriteRenderer))]
public class ActiveSpeaker : Interactable
{
    //Name of the speaker
    [Tooltip("The name of the speaker.")]
    public string speakerName = "Default";
    //The starting string used to initiate the yarn node
    [Tooltip("The starting string used to initiate the yarn node. Should be the name of the node.")]
    public string yarnStartNode = "Start";
    //The actual yarn diablogue file the speaker will read from
    [Tooltip("The yarn file the speaker will read from.")]
    public YarnProgram yarnDialogue;
    //A reference to the current activepseaker object which is talking.
    public static ActiveSpeaker ACTIVE_NPC { get; set; }
    //Set whether this speaker is being interacted with as an npc.
    public bool npcMode;
    //Reference to the objects sprite renderer
    public SpriteRenderer spriteInfo { get; private set;}
    //flag used to tell if this npc is talking.
    bool isTalking;

    //Initialize monobehaviour fields
    void Start()
    {
        if (this.gameObject.CompareTag("Player"))
        {
            Debug.LogWarning("ActiveSpeaker: This component should not be attached to the player");
            this.enabled = false;
        }
        else
        {
            spriteInfo = GetComponent<SpriteRenderer>();
            colliderInfo.enabled = false;
            if(MenuManager.DIALOGUE_MENU && yarnDialogue != null)
            {
                MenuManager.DIALOGUE_MENU.dialogueRunner.Add(yarnDialogue);
            }
            else
            {
                Debug.LogWarning("ActiveSpeaker: The following speaker failed to load their yarnDialogue: " + 
                this.gameObject.name);
            }
            colliderInfo.enabled = true;
        }
    }

    public override void OnInteract()
    {
        if (npcMode)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                player.SendMessage("StartTalking", this);
            }
            MenuManager.INTERACT_MENU.Hide();
        }
    }

    //Check if the player is in range to talk to this speaker.
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (this.npcMode && collider.CompareTag("Player"))
        {
            ShowIndicator(true);
            SetPotentialInteractable(true, collider.gameObject);
        }
    }

    //If the player has left the range, this speaker is no longer the active speaker.
    protected override void OnTriggerExit2D(Collider2D collider)
    {
        if (this.npcMode && collider.CompareTag("Player"))
        {
            ShowIndicator(false);
            SetPotentialInteractable(false, collider.gameObject);
        }
    }

    protected override void ShowIndicator(bool value)
    {
        if (npcMode)
        {
            interactIndicator.SetActive(value);
        }
    }
    public void SetIsTalking(bool value)
    {
        isTalking = value;
        ShowIndicator(!value);
    }
}
