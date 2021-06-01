using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Document Link: https://docs.google.com/document/d/114CKhs4LBbA6-xtxwUMiokUPrs0vNGiLOeBU10qb5D4/edit?usp=sharing
[RequireComponent(typeof(Camera))]
public class HubCamera : BaseCamera
{
    /*MoveCamera will move the camera to a specified position.
    The position in this case will be the players transform by default.
    If there is point of interest close to the player, the camera will move to 
    a position between the player and the POI.
    The cameras movement is smoothed.*/
    protected override void MoveCamera()
    {
        Vector3 cameraPosition = GetCenterPos();
        cameraPosition = FocusCamOnChatBubble(cameraPosition);
        if (!closeToPOI)
        {
            cameraPosition = cameraPosition + new Vector3(offset.x, offset.y, cameraPosition.z);
        }
        cameraPosition = BoundCamera(cameraPosition);
        this.gameObject.transform.position = Vector3.SmoothDamp(this.gameObject.transform.position, 
            new Vector3(cameraPosition.x, cameraPosition.y, this.gameObject.transform.position.z), 
            ref velocity, cameraSmoothRate);
    }

    /*GetCenterPos well fetches the position the camera should be following.
    It uses Bounds to mediate the camera if there are multiple positions that need to be accounted for.
    It will check if a player is close to any POI's and adjust the camera's position based on 
    how close a player is to a POI.
    This function is utilized by MoveCamera.*/
    protected override Vector3 GetCenterPos()
    {
        if (!mainTargetTransform)
        {
            return Vector3.zero;
        }
        var bounds = new Bounds(mainTargetTransform.position, Vector3.zero);
        bounds.Encapsulate(mainTargetTransform.position);
        closeToPOI = false;
        if (!ignoreTargetPOIs)
        {
            float closestVal = zoomTriggerDistance;
            for (int i = 0; i < targetPOIs.Count; i++)
            {
                float distToPOI = Vector2.Distance(new Vector2(mainTargetTransform.position.x, mainTargetTransform.position.y),
                                                    new Vector2(targetPOIs[i].position.x, targetPOIs[i].position.y));
                if (distToPOI <= zoomTriggerDistance && distToPOI < closestVal)
                {
                    closeToPOI = true;
                    closestVal = distToPOI;
                    closestPOI = targetPOIs[i].position;
                }
            }
            if (closeToPOI)
            {
                bounds.Encapsulate(closestPOI);
            }
        }
        return bounds.center;
    }
}
