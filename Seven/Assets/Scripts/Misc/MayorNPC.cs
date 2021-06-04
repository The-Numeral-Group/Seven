using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MayorNPC : MonoBehaviour
{
    private int nextDialogueNum = 2;
    public Animator mayorAnimator;

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
        this.gameObject.GetComponent<ActiveSpeaker>().yarnStartNode = "Mayor.Opening.Dialogue" + nextDialogueNum;
        MenuManager.DIALOGUE_MENU.StartDialogue(this.gameObject);
        nextDialogueNum++;
    }

    public void playTalkAnimation()
    {
        mayorAnimator.SetBool("Mayor_Talk", true);
    }

    public void playIdleAnimation()
    {
        mayorAnimator.SetBool("Mayor_Talk", false);
    }

    public void changeChatBoxYOffset(float value)
    {
        //Debug.Log(this.gameObject.GetComponent<ActiveSpeaker>().chatBoxOffset);
        this.gameObject.GetComponent<ActiveSpeaker>().chatBoxOffset.y = value;
    }
}
