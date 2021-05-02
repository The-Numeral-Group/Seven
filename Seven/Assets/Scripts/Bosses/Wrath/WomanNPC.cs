using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WomanNPC : MonoBehaviour
{
    private int nextDialogueNum = 1;

    public string endingType;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(startDialogueDelay());
    }

    private IEnumerator startDialogueDelay()
    {
        yield return new WaitForSeconds(0.1f);
        if (this.gameObject.GetComponent<ActiveSpeaker>().npcMode == false)
        {
            MenuManager.DIALOGUE_MENU.StartDialogue(this.gameObject);
        }
    }

    public void playNextDialogue()
    {
        this.gameObject.GetComponent<ActiveSpeaker>().yarnStartNode = "Woman." + endingType + ".Dialogue" + nextDialogueNum;
        MenuManager.DIALOGUE_MENU.StartDialogue(this.gameObject);
        nextDialogueNum++;
    }
}
