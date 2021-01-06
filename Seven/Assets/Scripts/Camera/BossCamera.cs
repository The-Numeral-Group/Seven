using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//CREDIT FOR THE TEMPLATE CODE GOES TO BRACKEYS
//https://www.youtube.com/watch?v=aLpixrPvlB
public class BossCamera : MonoBehaviour
{
    public Transform playerTransform;
    public List<GameObject> targets;
    public float maxDistance = 50f;
    public Vector2 offset = new Vector2(0, 2);
    public float smoothTime = 0.5f;
    private Vector3 velocity;
    private Vector2 currOffset;

    void FixedUpdate()
    {
        MoveCamera();
    }
    
    /*
    MoveCamera adjusts the cameras position.
    By default it will focus on the player.
    If there are targets close to the player, the camera will mediate a position between the player and all the targets.
    The cameras movement is smoothed.
    */
    private void MoveCamera()
    {
        this.currOffset = this.offset;
        Vector3 centerPoint = GetCenterPos();
        Vector2 newPosition = new Vector2(centerPoint.x, centerPoint.y) + this.currOffset;
        this.gameObject.transform.position = Vector3.SmoothDamp(this.gameObject.transform.position, new Vector3(newPosition.x, newPosition.y, this.gameObject.transform.position.z),
                                                    ref this.velocity, this.smoothTime);
    }

    /*
    GetCenterPos returns a position that is used to inform where the camera should move to.
    That position by default is the players transform.
    If there are targets close to the player the camera will go to position that gives view of the specified targets and the player.
    This function is utilized by MoveCamera
    */
    private Vector3 GetCenterPos()
    {
        var bounds = new Bounds(this.playerTransform.position, Vector3.zero);
        bounds.Encapsulate(this.playerTransform.position);
        if (this.targets.Count > 0)
        {
            for (int i = 0; i < this.targets.Count; i++)
            {
                if (!targets[i].activeSelf)
                {
                    continue;
                }
                float targetDist = Vector2.Distance(new Vector2(this.playerTransform.position.x, this.playerTransform.position.y),
                                                    new Vector2(this.targets[i].transform.position.x, this.targets[i].transform.position.y));
                if (targetDist < this.maxDistance)
                {
                    bounds.Encapsulate(targets[i].transform.position);
                    this.currOffset = Vector2.zero;
                }
            }
        }
        return bounds.center;
    }
}
