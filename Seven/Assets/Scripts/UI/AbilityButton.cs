using UnityEngine.UI;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Button))]
public class AbilityButton : MonoBehaviour
{
    //set everything via inspector
    public Button abilityButton;
    public Image sourceImage;
    public Slider coolDownSlider;
    public ActorAbility TOD;
    public ActorAbility AOS;
    public Sprite TODImage;
    public Sprite AOSImage;
    public ActorAbility selectedAbility { get; set;}
    IEnumerator CooldownRoutine;

    void Awake()
    {
        //abilityButton = GetComponent<Button>();
        CooldownRoutine = AbilityCooldown(0f);
    }

    public void SetSelectedAbility(bool sinCommitted)
    {
        if (sinCommitted == true)
        {
            selectedAbility = AOS;
            sourceImage.sprite = AOSImage;
        }
        else
        {
            sourceImage.sprite = TODImage;
            selectedAbility = TOD;
        }
    }

    //public function to initiate cooldown feedback on the ability menu.
    public void StartCooldown(float time)
    {
        StopCoroutine(CooldownRoutine);
        coolDownSlider.value = 1f;
        CooldownRoutine = AbilityCooldown(time);
        StartCoroutine(CooldownRoutine);
    }

    //Adjust the sliders position to emulate a box being filled in.
    IEnumerator AbilityCooldown(float time)
    {
        float incrementValue = 0.1f;
        for (float i = 0; i < time; i += incrementValue)
        {
            yield return new WaitUntil(() => MenuManager.PAUSE_MENU.game_is_paused == false);
            //Debug.Log(MenuManager.PAUSE_MENU.game_is_paused);
            yield return new WaitForSecondsRealtime(incrementValue);
            coolDownSlider.value = 1 - (i / time);
        }
        coolDownSlider.value = 0f;
    }
}
