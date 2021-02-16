using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractHitbox : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "InteractiveObject")
        {
            collider.GetComponent<InteractiveObject>().Invoke();
        }
    }
}
