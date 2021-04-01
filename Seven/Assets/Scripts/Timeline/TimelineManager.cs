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

    public void loopFromStartTimeline()
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
    }

    public void setLoop(bool newLoop)
    {
        this.loop = newLoop;
    }
    
    public void resumeTimeline()
    {
        director.Resume();
    }

    public void loadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

}
