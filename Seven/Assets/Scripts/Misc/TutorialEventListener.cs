using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialEventListener : MonoBehaviour
{
    public GameObject gate;

    public bool objectSwitch;

    public bool sceneTransition;

    public string sceneToLoad;

    public bool mayorDialogue;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only collide with player
        if (collision.gameObject.tag == "Player")
        {
            if(objectSwitch)
            {
                // close gate
                gate.SetActive(true);

                // one time use
                this.gameObject.SetActive(false);
                return;
            }
            if (sceneTransition)
            {
                SceneManager.LoadScene(sceneToLoad);
                return;
            }
            if (mayorDialogue)
            {
                var mayorObject = GameObject.Find("Mayor");
                MenuManager.DIALOGUE_MENU.StartDialogue(mayorObject);
            }
        }
    }
}
