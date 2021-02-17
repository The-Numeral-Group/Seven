using UnityEngine;
using UnityEngine.UI;

public class BattleUI : BaseUI
{
    //reference to the players health bar slide. should be set through inspector.
    [Tooltip("Reference to the players health bar slider ui object.")]
    public Slider playerSlider;
    //reference to the boss' health bar slider. should be set through the inspector.
    [Tooltip("Reference to the boss' health bar slider ui object.")]
    public Slider bossSlider;
    //refernce to the boss text label. Should be handled via inspector
    [Tooltip("Reference to the bosses text label.")]
    public Text bossText;
    //reference to the player
    ActorHealth playerHealth;
    //reference to the player object;
    Actor playerActor;
    //reference to the boss
    MultiActor bossMultiActor;
    //reference to current phase of the boss
    GameObject boss;
    //reference to the bosses health component
    ActorHealth bossHealth;

    void Start()
    {
        SetReferences();
    }

    void Update()
    {
        UpdateReferences();
        UpdateSlider(ref playerHealth, ref playerSlider);
        UpdateSlider(ref bossHealth, ref bossSlider);
        //IMPORTANT: Remove this in the future
        TemporaryDeathCheckFunction();
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
        var bObject = GameObject.FindGameObjectWithTag("Boss");
        if (!bObject)
        {
            Debug.LogWarning("BattleUI: Cannot locate the boss object in the scene.");
        }
        else
        {
            //a boss is expected to be a multi actor object
            bossMultiActor = bObject.GetComponentInChildren<MultiActor>();
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
        if (bossMultiActor && bossMultiActor.gameObject.transform.parent != boss)
        {
            boss = bossMultiActor.gameObject.transform.parent.gameObject;
            bossHealth = boss.GetComponent<ActorHealth>();
            if (!bossHealth)
            {
                bossSlider.gameObject.SetActive(false);
            }
            else
            {
                bossSlider.gameObject.SetActive(true);
                bossSlider.maxValue = bossHealth.maxHealth;
                bossSlider.value = bossHealth.currentHealth;
                bossText.text = boss.name;
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
    void TemporaryDeathCheckFunction()
    {
        if (!bossHealth || !playerHealth)
        {
            return;
        }
        if (bossHealth.currentHealth == 0f || playerHealth.currentHealth == 0f)
        {
            MenuManager.StartGameOver();
            this.gameObject.SetActive(false);
        }
    }
}
