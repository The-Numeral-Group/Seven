using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickRotation : MonoBehaviour
{
    //FIELDS---------------------------------------------------------------------------------------
    public enum RotationAxis {
        X,
        Y,
        Z
    }

    [Tooltip("Which axis the object should rotate around.")]
    public RotationAxis axis = RotationAxis.Z;

    /*[Tooltip("How fast the object should rotate in DEGREES PER SECOND. Set to a negative number" +
        " for counter-clockwise rotation.")]
    public float rotationRate = 360f;*/

    [Header("Tick Timings. The number of ticks is equal to tickPeriod / (tickFrequency + " + 
        " tickDuration).")]
    [Tooltip("How many SECONDS it should take for the rotating object" + 
        " to return to its original rotation.")]
    public float tickPeriod = 5f;

    [Tooltip("How many SECONDS are between tick starts (not accounting for how long" + 
        " the tick takes).")]
    public float tickGap = 0.5f;

    [Tooltip("How many SECONDS a single tick movement should last.")]
    public float tickDuration = 0.1f;

    [Tooltip("Should the object rotate counter-clockwise or not.")]
    public bool counterClockwise = false;

    //the global tickrate of 60 (it doesn't seem accessible as a readonly value)
    private const float simulationTickRate = 60f;

    //the amount of degrees in a circle
    private const float fullCircle = 360f;

    //transform of rotating object
    private Transform trans;

    //the vector object needed to perform the rotation
    private Vector3 rotationEulers;

    //the amount of ticks needed to complete a full rotation
    private int tickAmount;

    //calculated distance each tick needs to travel
    private float tickDistance;

    //calculated degrees-per-second of each tick transformation
    private float tickRate;



    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        //grab transform object
        trans = this.gameObject.transform;

        /*Calculating Private variables
        tick amount Period / Gap
        tick distance = 360 / Amount
        tick speed = Distance / Duration*/
        tickAmount = (int)(tickPeriod / tickGap);
        tickDistance = fullCircle / tickAmount;
        tickRate = (tickDistance / tickDuration * (counterClockwise ? 1 : -1)) / simulationTickRate;

        ///DEBUG
        Debug.Log("TickRotation: distance of " + tickDistance);
        ///DEBUG


        //create a rotation vector based off of the selected axis
        switch(axis){
            case RotationAxis.X:
                rotationEulers = new Vector3(tickDistance, 0f, 0f);
                break;

            case RotationAxis.Y:
                rotationEulers = new Vector3(0f, tickDistance, 0f);
                break;

            case RotationAxis.Z:
                rotationEulers = new Vector3(0f, 0f, tickDistance);
                break;

            default:
                rotationEulers = new Vector3(0f, 0f, 0f);
                Debug.LogWarning("TickRotation: Rotation Axis could not be set correctly");
                break;
        }

        //start the rotation!
        StartCoroutine(rotationFunction());
    }

    //controls the rotation
    IEnumerator rotationFunction()
    {  
        //we wait the full tickgap since there is no initial tick
        yield return new WaitForSeconds(tickGap);
        while(true)
        {
            //do the tick
            yield return tickTravel(trans, rotationEulers, tickDuration, tickRate);
            //then wait the gap, while subtracting time lost to the actual tick
            yield return new WaitForSeconds(tickGap - tickDuration);
        }
    }

    //does the actual tick motion
    IEnumerator tickTravel(Transform t, Vector3 rotation, float duration, float rate)
    {
        var effectTime = 0.0f;

        /*the amount of distance each simulation tick that must be travelled, calculated
        from the total needed rotation divided by the needed speed of the rotation*/
        var rotChange = rotation / rate;

        ///DEBUG
        Debug.Log("TickRotation: tick. Rotating by " + rotChange + "for full distance of " + rotation + " and rate of " + rate);
        ///DEBUG

        while(effectTime < duration)
        {
            //apply a divided rotation
            t.Rotate(rotChange, Space.Self);

            //increment effectTime
            effectTime += Time.deltaTime;

            //wait a bit
            yield return null;
        }
    }
}

