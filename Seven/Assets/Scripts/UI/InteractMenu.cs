using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
