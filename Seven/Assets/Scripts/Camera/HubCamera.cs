using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class HubCamera : BaseCamera
{
    //The defaultZoom level.
    [Tooltip("The default zoom level when the player is the only object in focus.")]
    public float defaultZoom = 40f;
    //The closest zoom the camera can perform
    [Tooltip("How far a camera can zoom in on a point of interest.")]
    public float minZoom = 17f;
    //The speed at which the camera will zoom in and out
    [Tooltip("How fast a camera can zoom in on a point of interest")]
    public float zoomSpeed = 5.0f;
    //Flag used to to let ZoomCamera know if zooming is required.
    bool closeToPOI;
    //Of all the points of interest close to the player, this variable holds the one which is closest.
    Vector3 closestPOI;

    //Initialize member variables.
    void Awake()
    {
        closeToPOI = false;
        closestPOI = Vector3.zero;
    }

    //Did you know FixedUpdate is called before each physics update?
    protected override void FixedUpdate()
    {
        MoveCamera();
        ZoomCamera();
    }

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
        this.gameObject.transform.position = Vector3.SmoothDamp(this.gameObject.transform.position, 
            new Vector3(cameraPosition.x, cameraPosition.y, this.gameObject.transform.position.z), 
            ref velocity, cameraSmoothRate);
    }

    /*ZoomCamera will soom the camera based on whether or not the player it close to a POI
    The zoom is done gradually using Linear Interpolation.*/
    private void ZoomCamera()
    {
        float newZoom;
        if (closeToPOI)
        {
            newZoom = Mathf.Lerp(cam.orthographicSize, minZoom, (zoomSpeed * Time.deltaTime));
        }
        else
        {
            newZoom = Mathf.Lerp(cam.orthographicSize, defaultZoom, (zoomSpeed * Time.deltaTime));
        }
        cam.orthographicSize = newZoom;
    }

    /*GetCenterPos well fetches the position the camera should be following.
    It uses Bounds to mediate the camera if there are multiple positions that need to be accounted for.
    It will check if a player is close to any POI's and adjust the camera's position based on 
    how close a player is to a POI.
    This function is utilized by MoveCamera.*/
    protected override Vector3 GetCenterPos()
    {
        if (!playerTransform)
        {
            return Vector3.zero;
        }
        var bounds = new Bounds(playerTransform.position, Vector3.zero);
        bounds.Encapsulate(playerTransform.position);
        closeToPOI = false;
        float closestVal = breakingDistance;
        for (int i = 0; i < targetPOIs.Count; i++)
        {
            float distToPOI = Vector2.Distance(new Vector2(playerTransform.position.x, playerTransform.position.y),
                                                new Vector2(targetPOIs[i].position.x, targetPOIs[i].position.y));
            if (distToPOI <= breakingDistance && distToPOI < closestVal)
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
        return bounds.center;
    }
}
