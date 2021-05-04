using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathNPC : MonoBehaviour
{
    private int nextDialogueNum = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void playNextDialogue()
    {
        this.gameObject.GetComponent<ActiveSpeaker>().yarnStartNode = "Wrath.Death.Dialogue" + nextDialogueNum;
        MenuManager.DIALOGUE_MENU.StartDialogue(this.gameObject);
        nextDialogueNum++;
    }
}
