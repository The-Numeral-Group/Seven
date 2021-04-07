using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Document Link: https://docs.google.com/document/d/1RP5uecWZjUolkAuBUVMLujq9ZjwQIjKne4fRoaTj0-s/edit?usp=sharing
//CREDIT FOR THE TEMPLATE CODE GOES TO BRACKEYS
//https://www.youtube.com/watch?v=aLpixrPvlB
[RequireComponent(typeof(Camera))]
public abstract class BaseCamera : MonoBehaviour
{
    //The transform of the for the main target the camera will follow
    [Tooltip("The main camera the target will follow. Will default to the player object.")]
    public Transform mainTargetTransform;
    //List handles all the elements the camera needs to be aware of during gameplay.
    [SerializeField]
    [Tooltip("All other points of interest (POI) the camera needs to be aware of aside from the player.")]
    protected List<Transform> targetPOIs = new List<Transform>();
    //Flag used to set if the camera should ignore points of interest
    [Tooltip("Flag used to notify if the camera should ignore points of interest. Set true for the"
    + " camera to only follow the main target.")]
    public bool ignoreTargetPOIs = false;
    //Offset the camera from it's centerPosition
    [Tooltip("How far to offset the camera.")]
    public Vector2 offset = new Vector2(0, 0);
    //Smooths the cameras movement
    [Tooltip("Smooth the cameras movement. Values ideally between 0 and 1.")]
    public float cameraSmoothRate = 0.5f;
    //Distance used by derived camera classes to manage the focus between player and points of interest.
    [Tooltip("Distance required between a targetPOI to the player in order affect the cameras focus.")]
    public float breakingDistance = 0f;
    //upper boundary for the camera
    [Tooltip("Upper Boundary of the camera.")]
    public float upperBound = float.MaxValue;
    //lower boundary for the camera
    [Tooltip("Lower Boundary of the camera.")]
    public float lowerBound = float.MinValue;
    //right boundary for the camera
    [Tooltip("Right Boundary of the camera.")]
    public float rightBound = float.MaxValue;
    //left boundary for the camera
    [Tooltip("Left Boundary of the camera.")]
    public float leftBound = float.MinValue;
    //Reference variable that is utilized by the SmoothDamp function.
    protected Vector3 velocity;
    //Reference to this objects camera component
    protected Camera cam;
    IEnumerator shakePointer;

    //Initialize monobehaviour fields.
    protected virtual void Start()
    {
        if (!mainTargetTransform)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                mainTargetTransform = player.transform;
            }
            else
            {
                Debug.Log("Camera: Camera does not have a player object to follow.");
                mainTargetTransform = null;
            }
        }
        cam = GetComponent<Camera>();
    }

    /*Handles dynamic camera action
    We use fixed update because regulare update results in stuttering.*/  
    protected virtual void FixedUpdate()
    {
        MoveCamera();
    }

    //Method used to add a transform to a cameras target points of interest.
    public virtual void AddTransform(Transform toAdd)
    {
        targetPOIs.Add(toAdd);
    }

    //How the camera should move. Functionality should be implemented on a class by class basis.
    protected abstract void MoveCamera();

    //How a camera gets its focus point. Functionality should be implemented on a class by class basis.
    protected abstract Vector3 GetCenterPos();

    /*Shakes the camera. 1st argument is how long the shake will occur, while the second is
    argument pertains to how long the shake lasts.*/
    public virtual void Shake(float duration, float magnitude)
    {
        if (shakePointer != null)
        {
            StopCoroutine(shakePointer);
        }
        shakePointer = ShakeCamera(duration, magnitude);
        StartCoroutine(shakePointer);
    }

    /*Coroutine used to actually shake the camera. It is called from Shake function*/
    IEnumerator ShakeCamera(float duration, float magnitude)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            this.gameObject.transform.position = 
                new Vector3(this.gameObject.transform.position.x + offsetX,
                this.gameObject.transform.position.y + offsetY,
                this.gameObject.transform.position.z);

            elapsed += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    /*adjust the cameras position to accomodate for any dialogue box on screen.
    Takes in the current camera focus point as an argument, then modifies it by bounding that point
    with the chat bubble position. Returns that calucated point as Vector3.*/
    protected virtual Vector3 FocusCamOnChatBubble(Vector3 currentBoundsPos)
    {
        var bounds = new Bounds(currentBoundsPos, Vector3.zero);
        if (MenuManager.DIALOGUE_MENU && MenuManager.DIALOGUE_MENU.dialogueRunner.gameObject.activeSelf)
        {
            Vector3 dialogBubblePos = 
                cam.ScreenToWorldPoint(MenuManager.DIALOGUE_MENU.chatBubble.localPosition);
            bounds.Encapsulate(dialogBubblePos);
        }
        return bounds.center;
    }

    //credit: https://www.youtube.com/watch?v=05VX2N9_2_4
    protected virtual Vector3 BoundCamera(Vector3 currentBoundsPos)
    {
        float currentBoundsX = currentBoundsPos.x;
        float currentBoundsY = currentBoundsPos.y;
        //credit for calculating the camera screen positional value: 
        //https://answers.unity.com/questions/923782/please-help-to-understand-orthographic-camera-size.html
        float screenHeight = Camera.main.orthographicSize * 2;
        float screenWidth = screenHeight * Screen.width / Screen.height;
        currentBoundsY = Mathf.Clamp(currentBoundsY, lowerBound + (screenHeight / 2), upperBound - (screenHeight / 2));
        currentBoundsX = Mathf.Clamp(currentBoundsX, leftBound + (screenWidth / 2), rightBound - (screenWidth / 2));
        return new Vector3(currentBoundsX, currentBoundsY, currentBoundsPos.z);
    }
}
