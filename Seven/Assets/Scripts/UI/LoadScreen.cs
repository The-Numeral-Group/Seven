using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//https://docs.unity3d.com/ScriptReference/AsyncOperation.html
//credit: https://www.youtube.com/watch?v=fxxoACKCWVo user GameDevHQ
public class LoadScreen : MonoBehaviour
{
    public Slider progressBar;
    // Start is called before the first frame update
    void Awake()
    {
        progressBar.value = 0f;
    }

    void Start()
    {
        StartCoroutine(LoadAsynchronously());
    }

    IEnumerator LoadAsynchronously()
    {
        yield return null;
        AsyncOperation levelToLoad = SceneManager.LoadSceneAsync(GameSettings.SCENE_TO_LOAD);
        while(!levelToLoad.isDone)
        {
            progressBar.value = levelToLoad.progress;
            yield return new WaitForEndOfFrame();
        }
    }
}
