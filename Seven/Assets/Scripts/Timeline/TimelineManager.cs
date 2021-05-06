using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

// Doc: https://docs.google.com/document/d/1t8toHhDSd4lvUUPEubfjjK5jV6X9huaLechlpcymaXY/edit
public class TimelineManager : MonoBehaviour
{
    public PlayableDirector director;

    public bool loop = true;

    public float[] loopTime;

    private int loopIt = 0;

    private BaseCamera cam;

    private void Start()
    {
        var camObjects = FindObjectsOfType<BaseCamera>();
        if (camObjects.Length > 0)
        {
            cam = camObjects[0];
        }
        else
        {
            Debug.LogWarning("Gluttony Crush: does not have access to a camera that can shake");
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

    /*public void loopFromStartTimeline()
    {
        if(loop)
        {
            director.time = director.initialTime;
        }
    }

    public void loopfromSpecificTimeline(float time)
    {
        if(loop)
        {
            director.time = (double)time;
        }
    }*/

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

    public void loadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void cameraShake()
    {
        cam.Shake(2.0f, 0.2f);
    }
}
