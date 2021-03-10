using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostKnightNPC : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.GetComponent<ActiveSpeaker>().npcMode == false)
        {
            MenuManager.DIALOGUE_MENU.StartDialogue(this.gameObject);
        }
    }

    public void playNextDialogue()
    {
        this.gameObject.GetComponent<ActiveSpeaker>().yarnStartNode = "GhostKnight.Opening.Dialogue2";
        MenuManager.DIALOGUE_MENU.StartDialogue(this.gameObject);
    }
}
