using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UITrackTarget : MonoBehaviour
{
    public GameObject target;
    public RectTransform canvasTransform;
    public RectTransform myTransform;
    void FixedUpdate()
    {
        Vector2 viewPortPosition = 
            Camera.main.WorldToViewportPoint(target.transform.position);
        Vector2 proportionalPosition = new Vector2(
            viewPortPosition.x * canvasTransform.sizeDelta.x,
            viewPortPosition.y * canvasTransform.sizeDelta.y);
        Debug.Log(proportionalPosition);
        myTransform.localPosition = proportionalPosition;
    }
}
