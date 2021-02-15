using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
public class TimelineManager : MonoBehaviour
{
    public PlayableDirector director;
    
    void pauseTimeline()
    {
        director.Pause();
    }

    public void resumeTimeline()
    {
        director.Resume();
    }

    public void nextScene()
    {
        SceneManager.LoadScene("Tutorial");
    }
}
