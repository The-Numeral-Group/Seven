using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Activespeaker is a class you can attach to any actor you want to be able to talk to the player.
DO NOT ATTACH THIS OBJECT TO THE PLAYER*/
//Credit for this concept: https://www.youtube.com/watch?v=CJu0ObGDQHY&t=317s
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ActiveSpeaker : MonoBehaviour
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
    //A reference to the the most valid talking target for the player
    public static ActiveSpeaker ACTIVE_NPC { get; set; }
    //Set whether this speaker is being interacted with as an npc.
    public bool npcMode;
    //Offset the chat indicator object from the speaker
    [Tooltip("How much you want to offset the chat indicator from the speaker.")]
    public Vector2 offset = new Vector2(0, 5);
    //Reference to the objects sprite renderer
    public SpriteRenderer spriteInfo { get; private set;}
    //A gameobject sprite that shows the npc can be talked with.
    [SerializeField]
    GameObject chatIndicatorPrefab = null;
    //The instantiated chat object from the prefab
    GameObject chatIndicator;
    //flag used to tell if this npc is talking.
    bool isTalking;

    //Initialize monobehaviour fields
    void Awake()
    {
        if (this.gameObject.CompareTag("Player"))
        {
            Debug.LogWarning("ActiveSpeaker: This component should not be attached to the player");
            this.enabled = false;
        }
        else if (!chatIndicatorPrefab)
        {
            Debug.LogWarning("ActiveSpeaker: No gameobject assigned to chatIndicator.");
            this.enabled = false;
        }
        else
        {
            chatIndicator = Instantiate(chatIndicatorPrefab, this.gameObject.transform.position,
                Quaternion.identity);
            chatIndicator.transform.parent = this.gameObject.transform;
            chatIndicator.transform.localPosition = new Vector3(offset.x, offset.y, 
                chatIndicator.transform.localPosition.z);
            chatIndicator.SetActive(false);

            spriteInfo = GetComponent<SpriteRenderer>();
        }
    }

    void Start()
    {
	if(MenuManager.DIALOGUE_MENU && yarnDialogue != null)
	{
	    MenuManager.DIALOGUE_MENU.dialogueRunner.Add(yarnDialogue);
	}
	else
	{
	    Debug.LogWarning("ActiveSpeaker: The following speaker failed to load their yarnDialogue: " + 
	    this.gameObject.name);
        }
    }

    //Check if the player is in range to talk to this speaker.
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (this.npcMode && collider.CompareTag("Player"))
        {
            SetChatIndicator(true);
            SetActiveSpeaker(true);
        }
    }

    //If the player has left the range, this speaker is no longer the active speaker.
    void OnTriggerExit2D(Collider2D collider)
    {
        if (this.npcMode && collider.CompareTag("Player"))
        {
            SetChatIndicator(false);
            SetActiveSpeaker(false);
        }
    }

    void SetChatIndicator(bool value)
    {
        if (npcMode)
        {
            chatIndicator.SetActive(value);
        }
    }

    void SetActiveSpeaker(bool value)
    {
        ACTIVE_NPC = value ? this : null;
    }

    public void SetIsTalking(bool value)
    {
        isTalking = value;
        SetChatIndicator(!value);
    }
}
