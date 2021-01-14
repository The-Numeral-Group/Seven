using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CREDIT: https://www.youtube.com/watch?v=Y8nOgEpnnXo&t=171s&ab_channel=Brackeys
// The following codes are from this video.
[RequireComponent(typeof(Camera))]
public class ShakeCamera : MonoBehaviour
{
    public Camera mainCam;
    float shakeAmount = 0;
    void Awake()
    {
        if (mainCam == null)
        {
            this.mainCam = Camera.main;
        }
    }

    public void CameraShake(float amt, float length)
    {
        this.shakeAmount = amt;
        InvokeRepeating("BeginShake", 0, 0.01f);
        Invoke("StopShake", length);
    }
    void BeginShake()
    {
        if (this.shakeAmount > 0)
        {
            Vector3 camPos = mainCam.transform.position;

            float offsetX = Random.value * this.shakeAmount * 2 - this.shakeAmount;
            float offsetY = Random.value * this.shakeAmount * 2 - this.shakeAmount;

            camPos.x += offsetX;
            camPos.y += offsetY;

            this.mainCam.transform.position = camPos;

        }
    }

    void StopShake()
    {
        CancelInvoke("BeginShake");
        this.mainCam.transform.localPosition = Vector3.zero;
    }
}
