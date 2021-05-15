using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndulgenceAttackListener : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "Player Weapon")
        {
            this.gameObject.transform.parent.gameObject.GetComponent<IndulgenceOpening>().nextDialogue();
        }
    }
}
