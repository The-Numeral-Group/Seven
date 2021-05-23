﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubEventListener : MonoBehaviour
{
    public GameObject gameSaveManager;
    public GameObject timelineManager;

    private int bossDefeated = 0;
    private int numSinCorrupted = 0;

    private GameSaveManager gameSaveManagerScript;
    
    // Start is called before the first frame update
    void Start()
    {
        this.gameSaveManagerScript = gameSaveManager.GetComponent<GameSaveManager>();
        checkPlayerRespawn();
        checkBossProgress();
    }

    private void checkPlayerRespawn()
    {
        var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];
        if (this.gameSaveManagerScript.getBoolValue(19))
        {
            // Place playerObject next to pond
            playerObject.transform.position = new Vector2(0.0f, 4.24f);

            timelineManager.GetComponent<TimelineManager>().startTimeline();

            // Lock player's movement
            StartCoroutine(playerObject.GetComponent<Actor>().myMovement.LockActorMovement(14.0f));

            /*StartCoroutine(playerObject.GetComponent<Actor>().myMovement.LockActorMovement(3.0f));
            
            StartCoroutine(playerObject.GetComponent<Actor>().mySoundManager.muteSoundForDuration("PlayerRun", 3.0f));

            // Play Respawn animation
            PlayerAnimationHandler anim = playerObject.GetComponent<Actor>().myAnimationHandler as PlayerAnimationHandler;
            anim.animateRespawn();*/

            this.gameSaveManagerScript.setBoolValue(false, 19);
        }
    }

    private void checkBossProgress()
    {
        // Apathy Defeated?
        if (this.gameSaveManagerScript.getBoolValue(12)) bossDefeated++;
        // Ego Defeated?
        if (this.gameSaveManagerScript.getBoolValue(15)) bossDefeated++;
        // Indulgence Defeated?
        if (this.gameSaveManagerScript.getBoolValue(18)) bossDefeated++;

        // All bosses are defeated, check for sin.
        if (bossDefeated == 3) checkSinProgress();
    }

    private void checkSinProgress()
    {
        // Committed Apathy Sin?
        if (this.gameSaveManagerScript.getBoolValue(11)) numSinCorrupted++;
        // Committed Ego Sin?
        if (this.gameSaveManagerScript.getBoolValue(14)) numSinCorrupted++;
        // Committed Indulgence Sin?
        if (this.gameSaveManagerScript.getBoolValue(17)) numSinCorrupted++;

        if(numSinCorrupted > 0)
        {
            if(numSinCorrupted == 3)
            {
                // Play AoS Ending
                Debug.Log("Playing AoS Ending");
            }
            else
            {
                // Play HnH Ending
                Debug.Log("Playing HnH Ending");
            }
        }
        else
        {
            // Play ToD Ending (Wrath Fight)
            Debug.Log("Playing ToD Ending");
        }
    }

    
}
