using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class ApathyNPC : Interactable
{
    //FIELDS---------------------------------------------------------------------------------------
    //whether or not script should skip all the nonsense and get straight to the fight
    private static bool goToFightNow = false;

    [Tooltip("A reference to Apathy, so it can be activated/deactived properly")]
    public GameObject apathyObj;

    [Tooltip("The ability Object Apathy should drop when it dies.")]
    public GameObject abilityDropObject;

    [Tooltip("Props that should vanish from the arena when the fight starts.")]
    public GameObject prop;

    [Tooltip("The scene transition object that should only be present if the fight isn't" + 
        " currently happening.")]
    public GameObject sceneTransition;

    [Header("Music")]
    [Tooltip("The ambience that plays while not fighting Apathy if it is still alive.")]
    public AudioClip prefightAmbiance;

    [Tooltip("The ambience that plays while not fighting Apathy if it isn't alive anymore.")]
    public AudioClip postfightAmbiance;

    [Tooltip("The ambience that plays while fighting Apathy.")]
    public AudioClip fightAmbiance;

    [Header("Dialogue and Cutscenes")]
    [Tooltip("The gameObject that is going to be the speaker for Apathy." + 
        "Must have an activeSpeaker object")]
    public GameObject speakingObject;

    [Tooltip("The node of dialogue that Apathy starts with. The player cannot move during this" + 
        " dialogue.")]
    public string openingNode;

    //[Tooltip("The node of dialogue that Apathy contiunes with. The player can move during this" + 
        //" dialogue.")]
    //public string transitionNode;

    /*[Tooltip("The cutscene Scene to play when the dialogue ends. The fight will begin as soon as" + 
        " the cutscene is over.")]
    public string endCutscene;
    */

    [Tooltip("The cutscene to play when Apathy dies")]
    public string deathCutscene;

    //whether or not the player has started the fight in that particular instance
    //of the apathy room
    private bool fightStarted = false;

    //whether or not apathy has been defeated
    private bool fightCompleted = false;

    //whether or not the player walked out on Apathy without fighting it
    //Static to be remembered across instances (i.e. between room entries)
    private bool fightAbandoned = false;

    //the game save manager
    private GameSaveManager manager;

    //this object's audiosource
    private AudioSource audiosource;

    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        //save the gamestate manager
        manager = GameObject.Find("GameSaveManager")?.GetComponent<GameSaveManager>();

        //save the audio source
        audiosource = this.gameObject.GetComponent<AudioSource>();

        //if the fight should start now, skip all of this and throw hands
        if(ApathyNPC.goToFightNow)
        {
            EngageFight();
            return;
        }

        /*save whether or not the fight has been abandoned. If the sin flag is set, this is the
        case.*/
        fightAbandoned = manager.getBoolValue(11);

        /*save whether or not the fight has already been completed*/
        fightCompleted = manager.getBoolValue(12);

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

            //also do post-fight music
            SetMusic(postfightAmbiance);

            return;
        }
        else if(!fightAbandoned && fightCompleted)
        {
            //do the same, but don't drop the AOS
            //apathyObj.SetActive(false);
            Destroy(apathyObj);
            prop.SetActive(false);

            //place the TOD, but only if the player doesn't have it already
            if(!manager.getBoolValue(9))
            {
                var abilityPickup = Instantiate(abilityDropObject, Vector3.zero, 
                    Quaternion.identity).GetComponent<AbilityPickup>();
                abilityPickup.gameSaveManager = manager;
                abilityPickup.gameSaveAbilityPickupIndex = 9;
            }

            //also do post-fight music
            SetMusic(postfightAmbiance);

            return;
        }

        
        //save sin committal. We assume the player is going to leave, and change the value if
        //the player actually starts the fight.
        manager.setBoolValue(true, 11);

        //turn on prefight music
        SetMusic(prefightAmbiance);

        //turn on the corruption visual
        sceneTransition.transform.GetChild(0).gameObject.SetActive(true);
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
        if(!fightStarted)
        {
            fightAbandoned = true;  //fight is abandoned
            manager.setBoolValue(true, 12); //but apathy is still technically dead
        }
    }

    //What happens when this object is interacted with
    public override void OnInteract()
    {
        //start sloth's dialogue. Sloth's activator is passed as the on-end delegate, and movement
        //remains unlocked so the player can choose to leave
        //we offload this to the next frame to make sure the player's actor components
        //are loaded enough for the dialogue to work

        //however, if the fight's over, don't trigger it
        if(fightAbandoned || fightCompleted) return;

        StartCoroutine(DialogueOffsetStart());
    }

    //Turns on the Apathy fight. This instance handles prepping the arena
    public void ActivateSloth()
    {
        //turn off the interactable UI
        //this needs the player's collider specifically, so just fish that out
        base.OnTriggerExit2D(GameObject.FindWithTag("Player").GetComponent<Collider2D>());

        //save the sin uncommitment
        manager.setBoolValue(false, 11);
        //Remove the props from the room
        prop.SetActive(false);
        //turn on the fight music
        SetMusic(fightAmbiance);
        //flag the fight as started
        fightStarted = true;
        //and lower the flag to start the fight
        ApathyNPC.goToFightNow = false;

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
        /*//drop the TOD
        var abilityPickup = Instantiate(abilityDropObject, Vector3.zero, Quaternion.identity)
                .GetComponent<AbilityPickup>();
        abilityPickup.gameSaveManager = manager;
        abilityPickup.gameSaveAbilityPickupIndex = 9;

        //bring back the scene transition
        sceneTransition.SetActive(true);

        //but turn off the corruption effect
        sceneTransition.transform.GetChild(0).gameObject.SetActive(false);

        //turn on the postfight music
        SetMusic(postfightAmbiance);*/

        //mark the fight as completed
        //ApathyNPC.fightCompleted = true;
        manager.setBoolValue(true, 12);

        //play the death cutscene
        GameObject.Find("TimelineManager").SendMessage("loadScene", deathCutscene);

    }

    //Starts dialogue on the next frame. Can't be anonymous because a yield is used
    IEnumerator DialogueOffsetStart()
    {
        yield return null;

        Debug.Log("ApathyNPC: starting dialogue...");

        //set the first node of ActiveSpeaker
        var activeSpeak = speakingObject.GetComponent<ActiveSpeaker>();

        //set its node to be the starting node
        activeSpeak.yarnStartNode = openingNode;

        //Tell the dialogue what to do when the first node is finished
        /*System.Action dialoguePartTwo = new System.Action( () =>
        {
            //Switch which node should be used for the dialogue menu
            activeSpeak.yarnStartNode = transitionNode;

            /*And start the new dialogue which begins the transition cutscene and flags this class
            to skip all the dialogue and whatever and just start fighting*
            ApathyNPC.goToFightNow = true;
            MenuManager.DIALOGUE_MENU.StartDialogue(
                speakingObject,
                new DialogueMenu.TestDelegate( () => EngageFight() ),
                false
            );
        });*/

        //Tell the dialogue to play the prefight cutscene
        System.Action goToCutscene = new System.Action( () =>
        {
            //flag the next load of this script to throw hands
            ApathyNPC.goToFightNow = true;

            //and play the cutscene
            //GameObject.Find("TimelineManager").SendMessage("loadScene", cutscene);
            
        });

        //start the dialogue where the player can't move
        MenuManager.DIALOGUE_MENU.StartDialogue(
            speakingObject, 
            new DialogueMenu.TestDelegate(goToCutscene),
            false
        );
    }

    [YarnCommand("EZPlayerUnlock")]
    public void EZPlayerUnlock()
    {
        //Debug.Log("ApathyNPC: Unlocking player movement");
        //MenuManager.DIALOGUE_MENU.SetupPlayer(false);
    }

    //Immediately switches the audio source to start playing this clip
    void SetMusic(AudioClip clip)
    {
        audiosource.Stop();
        audiosource.clip = clip;
        audiosource.Play();
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
