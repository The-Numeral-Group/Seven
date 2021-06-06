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

            // Lock player
            StartCoroutine(lockPlayer(12.0f));

            this.gameSaveManagerScript.setBoolValue(false, 19);
        }
    }

    private IEnumerator lockPlayer(float duration)
    {
        this.playerObject.GetComponent<PlayerActor>().playerInput.SwitchCurrentActionMap("UI");
        yield return new WaitForSeconds(duration);
        this.playerObject.GetComponent<PlayerActor>().playerInput.SwitchCurrentActionMap("Player");
        yield return new WaitForSeconds(0.5f);
        if (MenuManager.BATTLE_UI)
        {
            MenuManager.BATTLE_UI.gameObject.SetActive(true);
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
                SceneManager.LoadScene("AoS");
            }
            else
            {
                // Play HnH Ending
                SceneManager.LoadScene("HnH");
            }
        }
        else
        {
            // Play ToD Ending (Wrath Fight)
            SceneManager.LoadScene("ToD");
        }
    }

    
}
