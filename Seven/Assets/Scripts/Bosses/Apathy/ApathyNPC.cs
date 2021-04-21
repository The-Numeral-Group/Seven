using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApathyNPC : Interactable
{
    [Tooltip("Props that should vanish from the arena when the fight starts.")]
    public GameObject prop;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnInteract()
    {
        //start sloth's dialogue. Sloth's activator is passed as the on-end delegate, and movement
        //remains unlocked so the player can choose to leave
        //we offload this to the next frame to make sure the player's actor components
        //are loaded enough for the dialogue to work
        StartCoroutine(DialogueOffsetStart());
    }

    public void ActivateSloth()
    {
        //Remove the props from the room
        prop.SetActive(false);
    }

    //Starts dialogue on the next frame. Can't be anonymous because a yield is used
    IEnumerator DialogueOffsetStart()
    {
        yield return null;

        MenuManager.DIALOGUE_MENU.StartDialogue(
            this.gameObject, 
            new DialogueMenu.TestDelegate( () => SendMessage("ActivateSloth") ), 
            false
        );
    }
}
