using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class HubCamera : MonoBehaviour
{
    public Transform playerTransform;
    public List<Transform> POI;
    public Vector2 offset = new Vector2(0, 10);
    public float minDistanceToPOI = 20f;
    public float maxZoom = 40f;
    public float minZoom = 17f;
    public float zoomSpeed = 5.0f;
    public float smoothTime = 0.5f;
    bool m_closeToPOI = false;
    private Vector3 velocity;
    private Camera cam;
    private Vector3 m_closestPOI;

    void Start()
    {
        this.cam = GetComponent<Camera>();
        m_closestPOI = Vector3.zero;
    }

    void FixedUpdate()
    {
        MoveCamera();
        ZoomCamera();
    }

    /*
    MoveCamera will move the camera to a specified position.
    The position in this case will be the players transform by default.
    If there is point of interest close to the player, the camera will move to a position between the player and the POI.
    The cameras movement is smoothed.
    */
    private void MoveCamera()
    {
        Vector3 cameraPosition = GetCenterPos();
        if (!this.m_closeToPOI)
        {
            cameraPosition = cameraPosition + new Vector3(this.offset.x, this.offset.y, cameraPosition.z);
        }
        this.gameObject.transform.position = Vector3.SmoothDamp(this.gameObject.transform.position, new Vector3(cameraPosition.x, cameraPosition.y, this.gameObject.transform.position.z), 
                                                ref this.velocity, this.smoothTime);
    }

    /*
    ZoomCamera will soom the camera based on whether or not the player it close to a POI
    The zoom is done gradually using Linear Interpolation.
    */
    private void ZoomCamera()
    {
        float newZoom;
        if (this.m_closeToPOI)
        {
            newZoom = Mathf.Lerp(this.cam.orthographicSize, this.minZoom, (this.zoomSpeed * Time.deltaTime));
        }
        else
        {
            newZoom = Mathf.Lerp(this.cam.orthographicSize, this.maxZoom, (this.zoomSpeed * Time.deltaTime));
        }
        this.cam.orthographicSize = newZoom;
    }

    /*
    GetCenterPos well fetches the position the camera should be following.
    It uses Bounds to mediate the camera if there are multiple positions that need to be accounted for.
    It will check if a player is close to any POI's and adjust the camera's position based on how close a player is to a POI
    This function is utilized by MoveCamera.
    */
    private Vector3 GetCenterPos()
    {
        var bounds = new Bounds(this.playerTransform.position, Vector3.zero);
        bounds.Encapsulate(this.playerTransform.position);
        this.m_closeToPOI = false;
        float closestVal = this.minDistanceToPOI;
        for (int i = 0; i < this.POI.Count; i++)
        {
            float distToPOI = Vector2.Distance(new Vector2(this.playerTransform.position.x, this.playerTransform.position.y),
                                                new Vector2(this.POI[i].position.x, this.POI[i].position.y));
            if (distToPOI <= this.minDistanceToPOI && distToPOI < closestVal)
            {
                this.m_closeToPOI = true;
                closestVal = distToPOI;
                this.m_closestPOI = this.POI[i].position;
            }
        }
        if (m_closeToPOI)
        {
            bounds.Encapsulate(m_closestPOI);
        }
        return bounds.center;
    }
}
