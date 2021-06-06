using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTutorial : MonoBehaviour
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("How long the tutorial should persist before it is destroyed")]
    public float tutorialDuration = 5f;

    [Tooltip("The UI to show for the first phase: picking up an ability")]
    public GameObject pickupUI;

    [Tooltip("The UI to show for the second phase: cycling an ability")]
    public GameObject cycleUI;

    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        //find the player and the gamesave manager
        var player = GameObject.Find("Player");
        var gamesave = GameObject.Find("GameSaveManager").GetComponent<GameSaveManager>();

        //if either are missing, skip the tutorial
        if(player == null || gamesave == null)
        {
            Debug.LogWarning("AbilityTutorial: player or gameSaveManager missing." +
                " Skipping tutorial");
            Destroy(this.gameObject);
            return;
        }

        //attach to the player
        this.gameObject.transform.parent = player.transform;

        //load the current progress of the tutorial
        var progress = gamesave.getFloatValue(20);

        //if the tutorial is complete, skip the tutorial
        if(progress >= 2f)
        {
            Destroy(this.gameObject);
            return;
        }

        //decide which part of the UI to show
        if(progress == 0f)
        {
            pickupUI.SetActive(true);
        }
        else
        {
            cycleUI.SetActive(true);
        }

        gamesave.setFloatValue(progress + 1f, 20);

        //Turn on self destruct timer
        StartCoroutine(SelfDestruct());
    }

    // Update is called once per frame
    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(tutorialDuration);
        Destroy(this.gameObject);
    }
}
