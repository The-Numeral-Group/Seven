using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WomanNPC : MonoBehaviour
{
    public int nextDialogueNum = 1;

    public string endingType;

    public bool playDialogueAtStart;

    // Start is called before the first frame update
    void Start()
    {
        if(playDialogueAtStart)
        {
            StartCoroutine(startDialogueDelay());
        }
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
