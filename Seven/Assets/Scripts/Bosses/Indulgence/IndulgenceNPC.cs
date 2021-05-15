using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndulgenceNPC : MonoBehaviour
{
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
