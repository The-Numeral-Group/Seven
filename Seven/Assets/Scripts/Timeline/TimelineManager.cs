using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using Yarn.Unity;

// Doc: https://docs.google.com/document/d/1t8toHhDSd4lvUUPEubfjjK5jV6X9huaLechlpcymaXY/edit
public class TimelineManager : MonoBehaviour
{
    public PlayableDirector director;

    public bool loop = true;

    public float[] loopTime;

    private int loopIt = 0;

    private BaseCamera cam;

    //the name of the last scene that was asyncronously loaded
    private string asyncScene;

    //and the object responsible for loading it
    private AsyncOperation sceneLoader;

    private void Start()
    {
        var camObjects = FindObjectsOfType<BaseCamera>();
        if (camObjects.Length > 0)
        {
            cam = camObjects[0];
        }
        else
        {
            Debug.LogWarning("TimelineManager: does not have access to a camera that can shake");
        }
    }
    public void goBackLoop()
    {
        if (loop)
        {
            if (loopIt >= loopTime.Length)
            {
                Debug.LogWarning("TimelineManager: loopIt has passed over loopTime.Length!");
            }
            else
            {
                director.time = loopTime[loopIt];
            }
        }
    }

    public void increaseLoopIt()
    {
        this.loopIt++;
    }

    public void setLoop(bool newLoop)
    {
        this.loop = newLoop;
    }

    public void startTimeline()
    {
        director.Play();
    }
    
    public void resumeTimeline()
    {
        director.Resume();
    }

    [YarnCommand("loadScene")]
    public void loadScene(string name)
    {
        if(asyncScene != null || name == asyncScene)
        {
            sceneLoader.allowSceneActivation = true;
        }
        else
        {
            SceneManager.LoadScene(name);
        }
    }

    public void asyncLoadScene(string name)
    {
        sceneLoader = SceneManager.LoadSceneAsync(name);
        asyncScene = name;
        sceneLoader.allowSceneActivation = false;
    }

    public void cameraShake()
    {
        cam.Shake(2.0f, 0.2f);
    }

    public void deathCutscene()
    {
        director.Play();
    }

    public void WrathCameraPosition()
    {
        cam.transform.position = new Vector3(0f, 11f, -10f);
    }
}
