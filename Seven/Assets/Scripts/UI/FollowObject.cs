using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reference: https://www.youtube.com/watch?v=7XVSLpo97k0&ab_channel=Epitome
public class FollowObject : MonoBehaviour
{
    public Transform lookAt;
    public Vector3 offset;

    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;   
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = cam.WorldToScreenPoint(lookAt.position + offset);

        if(transform.position != pos)
        {
            transform.position = pos;
        }
    }
}
