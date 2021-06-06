using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayerOnStart : MonoBehaviour
{
    public GameSaveManager gsm;
    public ActorHealth playerHealth;
    public int damageToApply = 3;
    // Start is called before the first frame update
    void Start()
    {
        //if sin committed
        if (!gsm.getBoolValue(17))
        {
            playerHealth.currentHealth = playerHealth.currentHealth > damageToApply ? playerHealth.currentHealth - damageToApply : 0;
        }
    }

}
