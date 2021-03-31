using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Doc: https://docs.google.com/document/d/1vGgvoBgfrjflh4DtdCMCZhVfULUenn3sO3fjYFKiiP4/edit
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
