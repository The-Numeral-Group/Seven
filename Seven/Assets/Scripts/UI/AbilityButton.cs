using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Button))]
public class AbilityButton : MonoBehaviour
{
    Button abilityButton;
    public ActorAbility TOD;
    public ActorAbility AOS;
    public Sprite TODImage;
    public Sprite AOSImage;
    public ActorAbility selectedAbility { get; set;}

    void Awake()
    {
        abilityButton = GetComponent<Button>();
    }

    public void SetSelectedAbility(bool sinCommitted)
    {
        if (sinCommitted)
        {
            selectedAbility = AOS;
            abilityButton.GetComponent<Image>().sprite = AOSImage;
        }
        else
        {
            abilityButton.GetComponent<Image>().sprite = TODImage;
            selectedAbility = TOD;
        }
    }
}
