using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

/*Menu responsible for running the dialogue menu
There should be at most one instance of this class per scene.*/
public class DialogueMenu : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    //reference to the yarn dialogue runner
    //public static DialogueRunner DIALOGUE_RUNNER;

    //Initialize monobehavior/component fields
    void Start()
    {
        /*Technically there should only exist one dialogue menu instance in the game.
        I didn't want to port over the 'singleton' scripts so I (Ram) figure I would just have the
        Dialogue runner reference be a static variable.
        if (!DialogueMenu.DIALOGUE_RUNNER)
        {
            Debug.Log("Fetching instance");
            DialogueMenu.DIALOGUE_RUNNER = dr;
        }*/
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
