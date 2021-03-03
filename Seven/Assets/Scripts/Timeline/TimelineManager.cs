using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
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

    public void setLoop(bool newLoop)
    {
        this.loop = newLoop;
    }
    
    void pauseTimeline()
    {
        director.Pause();
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
