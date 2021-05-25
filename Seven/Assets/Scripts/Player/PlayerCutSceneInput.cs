using UnityEngine.InputSystem;
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

    //https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.PlayerInput.html#UnityEngine_InputSystem_PlayerInput_currentControlScheme
    public void OnControlsChanged()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        //MenuManager.SwapControlUIImages(playerInput.currentControlScheme);
        SwapUIImage[] uiSwappers = Resources.FindObjectsOfTypeAll(typeof(SwapUIImage)) as SwapUIImage[];
        foreach(SwapUIImage uiSwapper in uiSwappers)
        {
            uiSwapper.SwapImage(playerInput.currentControlScheme);
        }
    }
}
