using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//This class is used as a contianer of callback functions to be used by the credit sequence.
public class Credits : MonoBehaviour
{
    public MainMenu mainMenu;
    public GameObject startMenu;
    public Button startMenuDefaultButton;
    public GameObject creditsContainer;
    public GameObject uiSelector;

    public void CreditsFinishSequence()
    {
        creditsContainer.SetActive(false);
        mainMenu.startingButton = startMenuDefaultButton;
        mainMenu.ShowSubMenu(startMenu);
        uiSelector.SetActive(true);
    }
}
