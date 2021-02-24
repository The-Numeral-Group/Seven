using System;
using UnityEngine;

//This class exists purely to rotate things around one axis forever
public class ConstantRotation : MonoBehaviour
{
    //FIELDS---------------------------------------------------------------------------------------
    public enum RotationAxis {
        X,
        Y,
        Z
    }

    [Tooltip("Which axis the object should rotate around.")]
    public RotationAxis axis = RotationAxis.Z;

    [Tooltip("How fast the object should rotate in DEGREES PER SECOND. Set to a negative number" +
        " for counter-clockwise rotation.")]
    public float rotationRate = 360f;

    private const float tickRate = 60f;

    private Transform trans;

    private Vector3 rotationEulers;

    private float rotationChange; 
    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        //grab transform object
        trans = this.gameObject.transform;

        //calculate the rotation amount relative to tickrate
        rotationChange = -(rotationRate / tickRate);

        //create a rotation vector based off of the selected axis
        switch(axis){
            case RotationAxis.X:
                rotationEulers = new Vector3(rotationChange, 0f, 0f);
                break;

            case RotationAxis.Y:
                rotationEulers = new Vector3(0f, rotationChange, 0f);
                break;

            case RotationAxis.Z:
                rotationEulers = new Vector3(0f, 0f, rotationChange);
                break;

            default:
                rotationEulers = new Vector3(0f, 0f, 0f);
                Debug.LogWarning("ConstantRotation: Rotation Axis could not be set correctly");
                break;
        }
    }

    // FixedUpdate is called once per simulation tick
    void FixedUpdate()
    {
        trans.Rotate(rotationEulers, Space.Self);
    }
}
