﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//CREDIT FOR THE TEMPLATE CODE GOES TO BRACKEYS
//https://www.youtube.com/watch?v=aLpixrPvlB
[RequireComponent(typeof(Camera))]
public abstract class BaseCamera : MonoBehaviour
{
    //List handles all the elements the camera needs to be aware of during gameplay.
    [SerializeField]
    [Tooltip("All other points of interest (POI) the camera needs to be aware of aside from the player.")]
    protected List<Transform> targetPOIs = new List<Transform>();
    //Offset the camera from it's centerPosition
    [Tooltip("How far to offset the camera.")]
    public Vector2 offset = new Vector2(0, 0);
    //Smooths the cameras movement
    [Tooltip("Smooth the cameras movement. Values ideally between 0 and 1.")]
    public float cameraSmoothRate = 0.5f;
    //Distance used by derived camera classes to manage the focus between player and points of interest.
    [Tooltip("Distance required between a targetPOI to the player in order affect the cameras focus.")]
    public float breakingDistance = 0f;
    //The transform of the player
    protected Transform playerTransform;
    //Reference variable that is utilized by the SmoothDamp function.
    protected Vector3 velocity;
    //Reference to this objects camera component
    protected Camera cam;
    IEnumerator shakePointer;

    //Initialize monobehaviour fields.
    protected virtual void Start()
    {
        var player = GameObject.FindGameObjectsWithTag("Player")?[0];
        if (player)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.Log("Camera: Camera does not have a player object to follow.");
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

    public virtual void Shake(float duration, float magnitude)
    {
        if (shakePointer != null)
        {
            StopCoroutine(shakePointer);
        }
        shakePointer = ShakeCamera(duration, magnitude);
        StartCoroutine(shakePointer);
    }
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
}