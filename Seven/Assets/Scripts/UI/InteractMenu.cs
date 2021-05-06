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
    RectTransform uiElementTextTransform = null;

    public Vector2 uiElementTextOffset { get; set;}

    [SerializeField]
    RectTransform uiElementImageTransform = null;
    public Vector2 uiElementImageOffset { get; set;}

    protected override void Awake()
    {
        base.Awake();
    }

    void FixedUpdate()
    {
        TrackTarget();
    }
    void TrackTarget()
    {
        if (target)
        {
            // Add Instruction Text (Image)
            Vector3 textOffset = uiElementTextOffset;
            Vector2 viewPortPosition = 
                Camera.main.WorldToViewportPoint(target.transform.position + textOffset);
            Vector2 proportionalPosition = new Vector2(
                viewPortPosition.x * canvasTransform.sizeDelta.x,
                viewPortPosition.y * canvasTransform.sizeDelta.y);
            uiElementTextTransform.localPosition = proportionalPosition;

            // Add L.Shift Image
            Vector3 imageOffset = uiElementImageOffset;
            viewPortPosition =
                Camera.main.WorldToViewportPoint(target.transform.position + imageOffset);
            proportionalPosition = new Vector2(
                viewPortPosition.x * canvasTransform.sizeDelta.x,
                viewPortPosition.y * canvasTransform.sizeDelta.y);
            uiElementImageTransform.localPosition = proportionalPosition;
        }
    }

    public override void Show()
    {
        this.gameObject.SetActive(true);
        TrackTarget();
    }
}
