using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartTickRotation : MonoBehaviour
{
    //FIELDS---------------------------------------------------------------------------------------
    public enum RotationAxis {
        X,
        Y,
        Z
    }

    [Tooltip("Which axis the object should rotate around.")]
    public RotationAxis axis = RotationAxis.Z;

    public bool counterClockwise = false;

    public int tickCount = 12;

    public float tickSpeed = 3f;

    public float tickGap = 3f;

    private float tickDuration;

    private const float fullCircle = 360f;

    private Vector3 rotationEulers;

    // Start is called before the first frame update
    void Start()
    {
        /*a tick lasts for its travel distance (360 / tickCount) divided by tickSpeed seconds*/
        tickDuration = (fullCircle / tickCount) / tickSpeed;
        Debug.Log($"SmartTickRotation: ({fullCircle} / {tickCount}) / {tickSpeed} = {tickDuration}");

        float realTickRate = -(tickSpeed / 60f);
        if(counterClockwise)
        {
            realTickRate *= -1f;
        }

        //create a rotation vector based off of the selected axis
        switch(axis){
            case RotationAxis.X:
                rotationEulers = new Vector3(realTickRate, 0f, 0f);
                break;

            case RotationAxis.Y:
                rotationEulers = new Vector3(0f, realTickRate, 0f);
                break;

            case RotationAxis.Z:
                rotationEulers = new Vector3(0f, 0f, realTickRate);
                break;

            default:
                rotationEulers = new Vector3(0f, 0f, 0f);
                Debug.LogWarning("SmartTickRotation: Rotation Axis could not be set correctly");
                break;
        }

        //start the rotation!
        StartCoroutine(RotationFunction());
    }

    IEnumerator RotationFunction()
    {
        while(true)
        {
            yield return StartCoroutine(Tick());

            yield return new WaitForSeconds(tickGap);
        }
    }

    IEnumerator Tick()
    {
        float effectTime = 0f;

        while(effectTime < tickDuration)
        {
            //apply a divided rotation
            this.gameObject.transform.Rotate(rotationEulers, Space.Self);

            //increment effectTime
            effectTime += Time.deltaTime;

            Debug.Log($"SmartTickRotation: {tickDuration - effectTime} remaining");

            //wait a bit
            yield return null;
        }
    }
}
