using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
//Document Link: https://docs.google.com/document/d/1xRoI-Sgd_mhcsRJRdVyNVtCo6J9Vg1foxXbZLyPI7GE/edit?usp=sharing
//Handles the battle ui elements of the game
public class BattleUI : BaseUI
{
    //reference to the players health bar slide. should be set through inspector.
    [Tooltip("Reference to the players health bar slider ui object.")]
    public Slider playerSlider;
    //reference to the player position scriptable object.
    //This is here for TemporaryDeathCheckFunction.
    //This will be moved once we use different way to check death.
    public VectorValue playerPos;
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
    //reference to the player actor;
    Actor playerActor;
    //reference to the player gameobject;
    GameObject playerObject;
    //reference to all the audio sources;
    private AudioSource[] allAudioSources;
    //Reference to health bar shake coroutine
    IEnumerator ShakePTR;
    Vector3 sliderOriginalPos;

    protected override void Awake()
    {
        base.Awake();
        bossList = new List<BossBar>();
        ShakePTR = ShakePlayerHealthRoutine(1f, 0.1f);
        sliderOriginalPos = playerSlider.transform.localPosition;
    }
    void Start()
    {
        SetReferences();
    }

    protected override void Update()
    {
        UpdateReferences();
        UpdateSlider(ref playerHealth, ref playerSlider);
        for(int i = 0; i < bossList.Count; i++)
        {
            BossBar bBar = bossList[i];
            UpdateSlider(ref bBar.bossHealth, ref bBar.bossSlider);
            //IMPORTANT: Remove this in the future
            TemporaryDeathCheckFunction(bBar, ref i);
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
            playerObject = pObject;
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
                    bBar.bossText.text = bBar.bossMultiActor.actorName;
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
    void TemporaryDeathCheckFunction(BossBar bBar,ref int index)
    {
        if (!bBar.bossHealth || !playerHealth)
        {
            return;
        }
        if (bBar.bossHealth.currentHealth == 0f)
        {
            StopAllAudio();
            bossList.Remove(bBar);
            Destroy(bBar.bossContainer);
            index--;
        }
    }

    // Stop all the audio when game is over.
    // Source: https://answers.unity.com/questions/194110/how-to-stop-all-audio.html
    //I made the function public
    public void StopAllAudio()
    {
        allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach (AudioSource audioS in allAudioSources) {
            audioS.Stop();
        }
    }

    public void ShakePlayerHealthBar()
    {
        if (ShakePTR != null)
        {
            StopCoroutine(ShakePTR);
        }
        playerSlider.transform.localPosition = sliderOriginalPos;
        ShakePTR = ShakePlayerHealthRoutine(1f, 1.5f);
        StartCoroutine(ShakePTR);
    }

    IEnumerator ShakePlayerHealthRoutine(float duration, float magnitude)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            playerSlider.transform.localPosition = 
                new Vector3(playerSlider.transform.localPosition.x + offsetX,
                playerSlider.transform.localPosition.y + offsetY,
                playerSlider.transform.localPosition.z);

            elapsed += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        playerSlider.transform.localPosition = sliderOriginalPos;
    }
}
