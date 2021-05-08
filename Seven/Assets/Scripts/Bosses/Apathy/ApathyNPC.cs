using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApathyNPC : Interactable
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("A reference to Apathy, so it can be activated/deactived properly")]
    public GameObject apathyObj;

    [Tooltip("The ability Object Apathy should drop when it dies.")]
    public GameObject abilityDropObject;

    [Tooltip("Props that should vanish from the arena when the fight starts.")]
    public GameObject prop;

    [Tooltip("The scene transition object that should only be present if the fight isn't" + 
        " currently happening.")]
    public GameObject sceneTransition;

    //whether or not apathy has been defeated
    public static bool fightCompleted = false;

    //whether or not the player walked out on Apathy without fighting it
    //Static to be remembered across instances (i.e. between room entries)
    public static bool fightAbandoned = false;

    //the game save manager
    private GameSaveManager manager;

    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        //save the gamestate manager
        manager = GameObject.Find("GameSaveManager")?.GetComponent<GameSaveManager>();

        if(fightAbandoned && !fightCompleted)
        {
            //remove both props and Sloth, and place the AOS if it is not already there
            //also turn of this object's collider to prevent talking to sloth
            this.GetComponent<Collider2D>().enabled = false;
            //apathyObj.SetActive(false);
            Destroy(apathyObj);
            prop.SetActive(false);
            var abilityPickup = Instantiate(abilityDropObject, Vector3.zero, Quaternion.identity)
                .GetComponent<AbilityPickup>();
            abilityPickup.gameSaveManager = manager;
            abilityPickup.gameSaveAbilityPickupIndex = 9;

            return;
        }
        else if(!fightAbandoned && fightCompleted)
        {
            //do the same, but don't drop the AOS
            //apathyObj.SetActive(false);
            Destroy(apathyObj);
            prop.SetActive(false);

            return;
        }

        
        //save sin committal. We assume the player is going to leave, and change the value if
        //the player actually starts the fight.
        manager.setBoolValue(true, 11);

    }

    // Update is called once per frame
    // Needs to be blank but present for cutscene purposes
    void Update()
    {
        
    }

    // OnDestroy is called when this object is destroyed
    void OnDestroy()
    {
        /*Gonna get kinda gimmicky here...
        When the scene unloads, this object will be destroyed. If fightCompleted
        is false when this is destroyed, that means the scene has been unloaded without the
        fight finishing, which means the player abandoned the fight...*/
        if(!fightCompleted)
        {
            fightAbandoned = true;
        }
    }

    //What happens when this object is interacted with
    public override void OnInteract()
    {
        //start sloth's dialogue. Sloth's activator is passed as the on-end delegate, and movement
        //remains unlocked so the player can choose to leave
        //we offload this to the next frame to make sure the player's actor components
        //are loaded enough for the dialogue to work
        StartCoroutine(DialogueOffsetStart());
    }

    //Turns on the Apathy fight. This instance handles prepping the arena
    public void ActivateSloth()
    {
        //save the sin uncommitment
        manager.setBoolValue(false, 11);
        //Remove the props from the room
        prop.SetActive(false);

        //Remove the scene transition
        sceneTransition.SetActive(false);

        //Disable this gameObject's collision
        this.gameObject.GetComponent<Collider2D>().enabled = false;
        
        //tape an observer to Apathy
        var observer = apathyObj.AddComponent<ApathyDeathObserver>();
        //and tell the observer what to do when apathy dies
        observer.deathDelegate = new System.Action( () => DisengageFight() );
    }

    //Dispatches activation Messages to both this object and Apathy
    public void EngageFight()
    {
        SendMessage("ActivateSloth");
        apathyObj.SendMessage("ActivateSloth");
    }

    //Performs arena-based actions that occur when Apathy dies
    public void DisengageFight()
    {
        //drop the TOD
        var abilityPickup = Instantiate(abilityDropObject, Vector3.zero, Quaternion.identity)
                .GetComponent<AbilityPickup>();
        abilityPickup.gameSaveManager = manager;
        abilityPickup.gameSaveAbilityPickupIndex = 9;

        //bring back the scene transition
        sceneTransition.SetActive(true);

        //mark the fight as completed
        ApathyNPC.fightCompleted = true;
    }

    //Starts dialogue on the next frame. Can't be anonymous because a yield is used
    IEnumerator DialogueOffsetStart()
    {
        yield return null;

        MenuManager.DIALOGUE_MENU.StartDialogue(
            this.gameObject, 
            new DialogueMenu.TestDelegate( () => EngageFight() ), 
            false
        );
    }
}

//Taped to Apathy so this class can easily tell when it dies without needing to check constantly
internal class ApathyDeathObserver : MonoBehaviour
{
    //the delegate to invoke when Apathy dies
    [System.NonSerialized]
    public System.Action deathDelegate = null;

    //METHODS--------------------------------------------------------------------------------------
    //What happens when the gameObject this is attached to dies.
    public void DoActorDeath()
    {
        //We just want to invoke the death delegate
        deathDelegate?.Invoke();
    }
}
