using UnityEngine;

//Class is used to let wrath phase 2 know they should perform the melee attack.
public class WrathMeleeTrigger : MonoBehaviour
{
    //If the player enters the range we set the flag
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("In Range");
            WrathP2Actor.targetInRange = true;
        }
    }

    //if the player leaves the range we set the flag
    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            WrathP2Actor.targetInRange = false;
        }
    }
}
