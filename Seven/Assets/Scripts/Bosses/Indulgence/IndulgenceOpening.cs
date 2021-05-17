using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IndulgenceOpening : MonoBehaviour
{ 
    private int nextDialogueNum = 0;

    public void nextDialogue()
    {
        this.gameObject.GetComponent<ActiveSpeaker>().yarnStartNode = "Indulgence.Opening.Dialogue" + nextDialogueNum;
        MenuManager.DIALOGUE_MENU.StartDialogue(this.gameObject);
        nextDialogueNum++;
    }

    public void checkDialogueProgress()
    {
        if(nextDialogueNum == 3)
        {
            SceneManager.LoadScene("IndulgenceBattlePhase1");
        }
    }

}
