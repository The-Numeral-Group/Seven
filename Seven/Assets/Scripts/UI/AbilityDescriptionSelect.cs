using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//https://docs.unity3d.com/2018.1/Documentation/ScriptReference/UI.Selectable.OnSelect.html
//Class is used as an interface to access the onselect callback selectable objects have.
//Thsi is being  used for the ability descriptions.
public class AbilityDescriptionSelect : MonoBehaviour, ISelectHandler
{
    public string bossName = "";
    public AbilitySubMenu abilitySubMenu;

    public void OnSelect(BaseEventData eventData)
    {
        abilitySubMenu.UpdateAbilityText(bossName);
    }
}
