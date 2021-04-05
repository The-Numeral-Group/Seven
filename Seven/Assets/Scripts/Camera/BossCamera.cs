using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Document Link: https://docs.google.com/document/d/1VN4WdN25J94vSFCjBBo6KT83T3Wy7_CpXnYDwjTfafo/edit?usp=sharing
public class BossCamera : BaseCamera
{
    //Vector used to apply offset if and only if the player is the sole focus of the camera.
    Vector2 currOffset;

    /*MoveCamera adjusts the cameras position.
    By default it will focus on the player.
    If there are targets close to the player, the camera will mediate a position between the player and all the targets.
    The cameras movement is smoothed.*/
    protected override void MoveCamera()
    {
        currOffset = offset;
        Vector3 centerPoint = GetCenterPos();
        centerPoint = FocusCamOnChatBubble(centerPoint);
        Vector2 newPosition = new Vector2(centerPoint.x, centerPoint.y) + currOffset;
        newPosition = BoundCamera(newPosition);
        this.gameObject.transform.position = Vector3.SmoothDamp(this.gameObject.transform.position, 
            new Vector3(newPosition.x, newPosition.y, this.gameObject.transform.position.z),
            ref velocity, cameraSmoothRate);
    }

    /*GetCenterPos returns a position that is used to inform where the camera should move to.
    That position by default is the players transform.
    If there are targets close to the player the camera will go to position that gives view 
    of the specified targets and the player.
    This function is utilized by MoveCamera*/
    protected override Vector3 GetCenterPos()
    {
        if (!mainTargetTransform)
        {
            return Vector3.zero;
        }
        var bounds = new Bounds(mainTargetTransform.position, Vector3.zero);
        bounds.Encapsulate(mainTargetTransform.position);
        if (targetPOIs.Count > 0 && !ignoreTargetPOIs)
        {
            for (int i = 0; i < targetPOIs.Count; i++)
            {
                if (targetPOIs[i] == null || !targetPOIs[i].gameObject.activeSelf)
                {
                    targetPOIs.RemoveAt(i);
                    i--;
                    continue;
                }
                float targetDist = Vector2.Distance(new Vector2(mainTargetTransform.position.x, 
                                                                mainTargetTransform.position.y),
                                                    new Vector2(targetPOIs[i].position.x, 
                                                                targetPOIs[i].position.y));
                if (targetDist < breakingDistance)
                {
                    bounds.Encapsulate(targetPOIs[i].position);
                    currOffset = Vector2.zero;
                }
            }
        }
        return bounds.center;
    }
}
