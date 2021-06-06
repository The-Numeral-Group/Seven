using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class ApathyNPC : Interactable
{
    //FIELDS---------------------------------------------------------------------------------------
    //whether or not script should skip all the nonsense and get straight to the fight
    private static bool goToFightNow = false;

    [Header("Props")]
    [Tooltip("A reference to Apathy, so it can be activated/deactived properly")]
    public GameObject apathyObj = null;

    [Tooltip("The ability Object Apathy should drop when it dies.")]
    public GameObject abilityDropObject = null;

    [Tooltip("Props that should vanish from the arena when the fight starts.")]
    public GameObject prop = null;

    [Tooltip("The scene transition object that should only be present if the fight isn't" + 
        " currently happening.")]
    public GameObject sceneTransition = null;

    [Header("Music")]
    [Tooltip("The ambience that plays while not fighting Apathy if it is still alive.")]
    public AudioClip prefightAmbiance = null;

    [Tooltip("The ambience that plays while not fighting Apathy if it isn't alive anymore.")]
    public AudioClip postfightAmbiance = null;

    [Tooltip("The ambience that plays while fighting Apathy.")]
    public AudioClip fightAmbiance = null;

    [Header("Dialogue and Cutscenes")]
    [Tooltip("The gameObject that is going to be the speaker for Apathy." + 
        "Must have an activeSpeaker object")]
    public GameObject speakingObject = null;

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

    [Header("Lights")]
    [Tooltip("The central light that provides most of the ambient light in the scene")]
    public UnityEngine.Experimental.Rendering.Universal.Light2D centralLight = null;

    [Tooltip("The intensity to make the light in the No-Sin end scene.")]
    public float centralLightWinIntensity = 0.45f;

    [Tooltip("The intensity to make the light in the Sin end scene.")]
    public float centralLightSinIntensity = 0.45f;

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
    //Reference to ActiveSpeaker
    ActiveSpeaker activeS;

    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        activeS = GetComponent<ActiveSpeaker>();
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

        //save that the fight has been started in some form
        manager.setBoolValue(false, 10);

        /*save whether or not the fight has been abandoned. If the sin flag is set, this is the
        case.*/
        fightAbandoned = manager.getBoolValue(11);

        /*save whether or not the fight has already been completed*/
        fightCompleted = manager.getBoolValue(12);

        //Sin End
        if(fightAbandoned)
        {
            //remove both props and Sloth, and place the AOS if it is not already there
            //also turn of this object's collider to prevent talking to sloth
            this.GetComponent<Collider2D>().enabled = false;
            
            //get rid of Apathy and count it as dead
            Destroy(apathyObj);
            manager.setBoolValue(true, 12);

            prop.SetActive(false);

            //adjust the lighting intensity
            centralLight.intensity = centralLightSinIntensity;

            var abilityPickup = Instantiate(abilityDropObject, Vector3.zero, Quaternion.identity)
                .GetComponent<AbilityPickup>();
            abilityPickup.gameSaveManager = manager;
            abilityPickup.gameSaveAbilityPickupIndex = 9;

            //also do post-fight music
            SetMusic(postfightAmbiance);

            return;
        }
        //No-Sin End
        else if(!fightAbandoned && fightCompleted)
        {
            //do the same, but don't drop the AOS
            //apathyObj.SetActive(false);
            Destroy(apathyObj);
            prop.SetActive(false);

            //adjust the lighting intensity
            centralLight.intensity = centralLightWinIntensity;

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
        //we actually can't make this assumption
        //manager.setBoolValue(true, 11);

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
    protected override void OnDestroy()
    {
        base.OnDestroy();
        /*Gonna get kinda gimmicky here...
        When the scene unloads, this object will be destroyed. If fightCompleted
        is false when this is destroyed, that means the scene has been unloaded without the
        fight finishing, which means the player abandoned the fight...*/
        if(!fightStarted && !ApathyNPC.goToFightNow && !fightCompleted)
        {
            fightAbandoned = true;  //fight is abandoned
            //save the sin flag
            manager.setBoolValue(true, 11);
            //and count apathy as dead
            manager.setBoolValue(true, 12);
            //manager.setBoolValue(true, 12); //but apathy is still technically dead
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
        if (!activeS.isTalking)
        {
            StartCoroutine(DialogueOffsetStart());
        }
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
        
        //lower the flag that starts the fight
        ApathyNPC.goToFightNow = false;

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

    //overrider interact ontriggers
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !activeS.isTalking)
        {
            ShowIndicator(true);
            SetPotentialInteractable(true, this.gameObject);
        }
    }

    /*On trigger stay used to choose between multiple interactable objects within range of the player.
    performs selection based on distance.*/
    protected override void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (Interactable.POTENTIAL_INTERACTABLE && Interactable.POTENTIAL_INTERACTABLE != this && !activeS.isTalking)
            {
                float myDistanceToPlayer = Vector2.Distance(other.transform.position, this.transform.position);
                float closestPotentialDistanceToPlayer = Vector2.Distance(other.transform.position, 
                                                            POTENTIAL_INTERACTABLE.transform.position);
                if (myDistanceToPlayer < closestPotentialDistanceToPlayer)
                {
                    ShowIndicator(true);
                    SetPotentialInteractable(true, this.gameObject);
                }
                else
                {
                    ShowIndicator(false);
                }
            }
            else if (!Interactable.POTENTIAL_INTERACTABLE && !activeS.isTalking)
            {
                ShowIndicator(true);
                SetPotentialInteractable(true, this.gameObject);
            }
            else if (Interactable.POTENTIAL_INTERACTABLE == this && activeS.isTalking)
            {
                ShowIndicator(false);
                SetPotentialInteractable(false, this.gameObject);
            }
        }
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
