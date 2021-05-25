using System.Collections;
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

    private GameObject playerObject;
    
    // Start is called before the first frame update
    void Start()
    {
        this.gameSaveManagerScript = gameSaveManager.GetComponent<GameSaveManager>();
        this.playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];
        checkPlayerRespawn();
        checkBossProgress();
    }

    private void checkPlayerRespawn()
    {
        if (this.gameSaveManagerScript.getBoolValue(19))
        {
            // Place playerObject next to pond
            playerObject.transform.position = new Vector2(0.0f, 4.24f);

            timelineManager.GetComponent<TimelineManager>().startTimeline();

            // Lock player's movement
            StartCoroutine(playerObject.GetComponent<Actor>().myMovement.LockActorMovement(14.0f));

            // Lock player's dodge
            StartCoroutine(lockPlayerAbility(14.0f));

            this.gameSaveManagerScript.setBoolValue(false, 19);
        }
    }

    private IEnumerator lockPlayerAbility(float duration)
    {
        this.playerObject.GetComponent<PlayerAbilityInitiator>().canDodge = false;
        this.playerObject.GetComponent<PlayerAbilityInitiator>().canAttack = false;
        this.playerObject.GetComponent<PlayerAbilityInitiator>().canUseAbility = false;
        yield return new WaitForSeconds(duration);
        this.playerObject.GetComponent<PlayerAbilityInitiator>().canDodge = true;
        this.playerObject.GetComponent<PlayerAbilityInitiator>().canAttack = true;
        this.playerObject.GetComponent<PlayerAbilityInitiator>().canUseAbility = true;
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
