using UnityEngine;
using UnityEngine.UI;

//Class used for the cheat menu within the pausemenu prefab
public class CheatSubMenu : SubMenu
{
    public Slider damageSlider;
    public Text damageText;

    public Slider healthSlider;
    public Text healthText;

    PlayerWeaponAbility pWA = null;
    ActorHealth playerHealth = null;

    void Start()
    {
        var pObject = GameObject.FindGameObjectWithTag("Player");
        if (pObject)
        {
            PlayerActor player = pObject.GetComponent<PlayerActor>();
            if (player)
            {
                pWA = player.GetComponent<PlayerWeaponAbility>();
                playerHealth = player.GetComponent<ActorHealth>();
            }
        }
    }

    public void UpdatePlayerDamage()
    {
        if (pWA)
        {
            pWA.SetDamage((int)damageSlider.value);
            damageText.text = damageSlider.value.ToString();
        }
    }

    public void UpdateHealth()
    {
        if (playerHealth)
        {
            playerHealth.currentHealth = (int)healthSlider.value;
            healthText.text = healthSlider.value.ToString();
        }
    }
}
