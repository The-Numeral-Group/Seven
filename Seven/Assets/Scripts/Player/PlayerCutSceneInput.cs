using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerCutSceneInput : MonoBehaviour
{
    public GameObject timeline;
    TimelineManager timelineManager;
    void Start()
    {
        this.timelineManager = timeline.GetComponent<TimelineManager>();
        if(timeline == null)
        {
            Debug.Log("PlayerCutSceneInput: Timeline Object is missing!");
        }
        if(timelineManager == null)
        {
            Debug.Log("PlayerCutSceneInput: Cannot find timelineManager componenet!");
        }
    }
    void Update()
    {
        if (timeline == null)
        {
            Debug.Log("PlayerCutSceneInput: Timeline Object is missing!");
        }
        if (timelineManager == null)
        {
            Debug.Log("PlayerCutSceneInput: Cannot find timelineManager componenet!");
        }
    }

    void OnDodge()
    {
        timelineManager.resumeTimeline();
    }
}
