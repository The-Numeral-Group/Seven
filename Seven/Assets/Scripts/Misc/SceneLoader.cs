using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Button startingButton;

    void Start()
    {
        if (startingButton)
        {
            startingButton.Select();
        }
    }

    public void OnShow()
    {
        this.gameObject.SetActive(true);
        if (startingButton)
        {
            startingButton.Select();
        }
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
