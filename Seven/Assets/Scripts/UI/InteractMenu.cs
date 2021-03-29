using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Document Link: https://docs.google.com/document/d/1OSd_Tu3ap9WTKyEv2gH52gNybbYtP4i9kkHwoPZQnsA/edit?usp=sharing
//Handles showing the prompt that lets the player know they can interact with something.
public class InteractMenu : BaseUI
{
    public GameObject target;
    [SerializeField]
    RectTransform canvasTransform = null;
    [SerializeField]
    RectTransform uiElementTransform = null;
    public Vector2 uiElementOffset {get; set;}

    protected override void Awake()
    {
        base.Awake();
    }
    void TrackTarget()
    {
        if (target)
        {
            Vector3 offset = uiElementOffset;
            Vector2 viewPortPosition = 
                Camera.main.WorldToViewportPoint(target.transform.position);// + offset);
            Vector2 proportionalPosition = new Vector2(
                viewPortPosition.x * canvasTransform.sizeDelta.x,
                viewPortPosition.y * canvasTransform.sizeDelta.y);
            uiElementTransform.localPosition = proportionalPosition;
        }
    }

    public override void Show()
    {
        this.gameObject.SetActive(true);
        TrackTarget();
    }
}
