using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BattleUI : BaseUI
{
    //reference to the players health bar slide. should be set through inspector.
    [Tooltip("Reference to the players health bar slider ui object.")]
    public Slider playerSlider;
    ///////
    class BossBar
    {
        public GameObject bossContainer;
        public Slider bossSlider;
        public Text bossText;
        public MultiActor bossMultiActor;
        public GameObject boss;
        public ActorHealth bossHealth;
    }
    //Reference to the ui elements containing the bosses information. Must be set through inspector
    [Tooltip("Reference to the boss' ui container.")]
    public GameObject bossContainers;
    //Reference to the prefab used to create the boss bars.
    [Tooltip("Reference to the prefab used to create boss health bars.")]
    public GameObject bossBar;
    List<BossBar> bossList;
    ///////
    //reference to the player
    ActorHealth playerHealth;
    //reference to the player object;
    Actor playerActor;
    //reference to all the audio sources;
    private AudioSource[] allAudioSources;

    void Awake()
    {
        bossList = new List<BossBar>();
    }
    void Start()
    {
        SetReferences();
    }

    void Update()
    {
        UpdateReferences();
        UpdateSlider(ref playerHealth, ref playerSlider);
        foreach(var bBar in bossList)
        {
            UpdateSlider(ref bBar.bossHealth, ref bBar.bossSlider);
            //IMPORTANT: Remove this in the future
            TemporaryDeathCheckFunction(bBar);
        }
    }

    //Set references for the player and boss actors
    public void SetReferences()
    {
        var pObject = GameObject.FindGameObjectWithTag("Player");
        if (!pObject)
        {
            Debug.LogWarning("BattleUI: Cannot locate the player object in the scene.");
        }
        else
        {
            //Object with player tag is expected to have actor component.
            playerActor = pObject.GetComponent<Actor>();
        }
        SetBossReference();
    }

    //used to set references to the boss object.
    public void SetBossReference()
    {
        var bObjects = GameObject.FindObjectsOfType<MultiActor>();
        if (bObjects.Length == 0)
        {
            Debug.LogWarning("BattleUI: Cannot locate the boss object in the scene.");
        }
        else
        {
            foreach(var bObject in bObjects)
            {
                BossBar bBar = new BossBar();
                GameObject bar = Instantiate(bossBar, bossContainers.transform);
                //a boss is expected to be a multi actor object
                //bossMultiActor = bObject.GetComponentInChildren<MultiActor>();
                bBar.bossContainer = bar;
                bBar.bossSlider = bar.GetComponentInChildren<Slider>();
                bBar.bossText = bar.GetComponentInChildren<Text>();
                bBar.bossMultiActor = bObject.GetComponentInParent<MultiActor>();
                bossList.Add(bBar);
            }
        }
    }

    //Update references to player and boss if need be.
    void UpdateReferences()
    {
        if (playerActor && !playerHealth)
        {
            playerHealth = playerActor.myHealth;
            playerSlider.maxValue = playerHealth.maxHealth;
            playerSlider.value = playerHealth.maxHealth;
        }
        foreach(var bBar in bossList)
        {
            if (bBar.bossMultiActor && bBar.bossMultiActor.gameObject.transform.parent != bBar.boss)
            {
                bBar.boss = bBar.bossMultiActor.gameObject.transform.parent.gameObject;
                bBar.bossHealth = bBar.boss.GetComponent<ActorHealth>();
                if (!bBar.bossHealth)
                {
                    bBar.bossContainer.SetActive(false);
                }
                else
                {
                    bBar.bossContainer.SetActive(true);
                    bBar.bossSlider.maxValue = bBar.bossHealth.maxHealth;
                    bBar.bossSlider.value = bBar.bossHealth.currentHealth;
                    bBar.bossText.text = bBar.boss.name;
                }
            }
        }
    }

    //Updates the slider values of an object. i.e. increments/decrements health
    void UpdateSlider(ref ActorHealth objectHealth, ref Slider uiSlider)
    {
        if (objectHealth)
        {
            uiSlider.value = objectHealth.currentHealth;
        }
    }

    //This function should be removed once we are done with stopping the game one player death
    //for testing purposes.
    void TemporaryDeathCheckFunction(BossBar bBar)
    {
        if (!bBar.bossHealth || !playerHealth)
        {
            return;
        }
        if (bBar.bossHealth.currentHealth == 0f)
        {
            StopAllAudio();
            SceneManager.LoadScene("Hub");

        }
        if (playerHealth.currentHealth == 0f)
        {
            StopAllAudio();
            MenuManager.StartGameOver();
            this.gameObject.SetActive(false);
        }
    }

    // Stop all the audio when game is over.
    // Source: https://answers.unity.com/questions/194110/how-to-stop-all-audio.html
    void StopAllAudio()
    {
        allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach (AudioSource audioS in allAudioSources) {
            audioS.Stop();
        }
    }
}
