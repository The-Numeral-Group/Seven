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
    public bool notInMainMenu;

    void Start()
    {
        if (notInMainMenu)
        {
            GetComponent<Animator>().SetBool("end", true);
        }
    }

    public void CreditsFinishSequence()
    {
        if (!notInMainMenu)
        {
            creditsContainer.SetActive(false);
            mainMenu.startingButton = startMenuDefaultButton;
            mainMenu.ShowSubMenu(startMenu);
            uiSelector.SetActive(true);
        }
        else
        {
            GameSettings.SCENE_TO_LOAD = "MainMenu";
            SceneManager.LoadScene("LoadScreen");
        }
    }

    public void OnMenu()
    {
        if (this.gameObject.activeSelf)
        {
            CreditsFinishSequence();
        }
    }
}
