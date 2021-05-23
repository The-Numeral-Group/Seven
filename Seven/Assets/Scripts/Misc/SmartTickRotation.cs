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

    public enum SpeedMode : int {
        fast = 20,
        normal = 10,
        slow = 5
    }

    [Tooltip("Which axis the object should rotate around.")]
    public RotationAxis axis = RotationAxis.Z;

    public bool counterClockwise = false;

    public int tickCount = 12;

    public float tickSpeed = 3f;

    public float tickGap = 3f;

    private float tickDuration;

    private const float fullCircle = 360f;

    private const float updateRate = 60f;

    private Vector3 rotationEulers;

    private ActorSoundManager soundManager;

    [Tooltip("The current speed the clock hand is running at. Please do not" +
        " adjust this at runtime.")]
    public SpeedMode speed = SpeedMode.normal;

    private string tickAudio = "normal_tick";

    private float baseTickGap;

    // Start is called before the first frame update
    void Start()
    {
        baseTickGap = tickGap;

        soundManager = this.gameObject.GetComponent<ActorSoundManager>();

        float realTickDistance = -fullCircle / tickCount;

        /*a tick lasts for its travel distance (360 / tickCount) divided by tickSpeed seconds*/
        tickDuration = (-realTickDistance) / tickSpeed;
        Debug.Log($"SmartTickRotation: ({fullCircle} / {tickCount}) / {tickSpeed} = {tickDuration}");

        float realTickRate = -(tickSpeed / updateRate);
        
        if(counterClockwise)
        {
            realTickRate *= -1f;
            realTickDistance *= -1f;
        }

        //create a rotation vector based off of the selected axis
        switch(axis){
            case RotationAxis.X:
                rotationEulers = new Vector3(realTickDistance, 0f, 0f);
                break;

            case RotationAxis.Y:
                rotationEulers = new Vector3(0f, realTickDistance, 0f);
                break;

            case RotationAxis.Z:
                rotationEulers = new Vector3(0f, 0f, realTickDistance);
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

        Vector3 originalRot = this.gameObject.transform.eulerAngles;
        Vector3 newRot = this.gameObject.transform.eulerAngles + rotationEulers;

        while(effectTime < tickDuration)
        {
            //apply a divided rotation
            //this.gameObject.transform.Rotate(rotationEulers, Space.Self);

            //lerp between the original rotation and the ideal rotation
            this.gameObject.transform.eulerAngles = 
                Vector3.Lerp(originalRot, newRot, effectTime/tickDuration);

            //increment effectTime
            effectTime += Time.deltaTime;

            //Debug.Log($"SmartTickRotation: {tickDuration - effectTime} remaining");

            //wait a bit
            yield return null;
        }

        //snap the remainder of the rotation into place
        this.gameObject.transform.eulerAngles = newRot;

        //play a tick sound
        float pitchVal = (int)speed / 10f;
        soundManager?.PlaySound(tickAudio, pitchVal, pitchVal);
    }

    public void SetSpeed(SpeedMode newMode)
    {
        Debug.Log($"SmartTickRotation: Adjusting speed to {newMode}");

        if(newMode == speed)
        {
            return;
        }
        else if(newMode == SpeedMode.slow)
        {
            soundManager?.PlaySound("slowdown_effect");
        }

        speed = newMode;
        tickGap = baseTickGap / ((int)speed / 10f);
    }
}
