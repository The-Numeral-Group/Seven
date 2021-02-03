using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Activespeaker is a class you can attach to any actor you want to be able to talk to the player.
DO NOT ATTACH THIS OBJECT TO THE PLAYER*/
//Credit for this concept: https://www.youtube.com/watch?v=CJu0ObGDQHY&t=317s
[RequireComponent(typeof(Collider2D))]
public class ActiveSpeaker : MonoBehaviour
{
    //The starting string used to initiate the yarn node
    [Tooltip("The starting string used to initiate the yarn node. Should be the name of the node")]
    public string yarnStartNode = "Start";
    //The actual yarn diablogue file the speaker will read from
    [Tooltip("The yarn file the speaker will read from")]
    public YarnProgram yarnDialogue;
    //A reference to the the most valid talking target for the player
    public static ActiveSpeaker ACTIVE_NPC { get; private set; }
    //Set whether this speaker is being interacted with as an npc.
    public bool npcMode {get; set;}
    //Offset the chat indicator object from the speaker
    [Tooltip("How much you want to offset the chat indicator from the speaker.")]
    public Vector2 offset = new Vector2(0, 5);
    //A gameobject sprite that shows the npc can be talked with.
    [SerializeField]
    GameObject chatIndicatorPrefab = null;
    //The instantiated chat object from the prefab
    GameObject chatIndicator;

    //Setup non monobehaviour member variables
    void Awake()
    {
        //Set to true for testing purposes
        npcMode = true;
    }

    //Initialize monobehaviour fields
    void Start()
    {
        if (this.gameObject.CompareTag("Player"))
        {
            Debug.LogWarning("ActiveSpeaker: This component should not be attached to the player");
            var activeSpeaker = this.gameObject.GetComponent<ActiveSpeaker>();
            activeSpeaker.enabled = false;
        }
        else if (!chatIndicatorPrefab)
        {
            Debug.LogWarning("ActiveSpeaker: No gameobject assigned to chatIndicator.");
        }
        else
        {
            chatIndicator = Instantiate(chatIndicatorPrefab, this.gameObject.transform.position,
                Quaternion.identity);
            chatIndicator.transform.parent = this.gameObject.transform;
            chatIndicator.transform.localPosition = new Vector3(offset.x, offset.y, 
                chatIndicator.transform.localPosition.z);
            chatIndicator.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (this.npcMode && collider.CompareTag("Player"))
        {
            SetChatIndicator(true);
            SetActiveSpeaker(true);
        }
    }

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
        chatIndicator.SetActive(value);
    }

    void SetActiveSpeaker(bool value)
    {
        ACTIVE_NPC = value ? this : null;
    }
}
