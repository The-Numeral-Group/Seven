using UnityEngine;

//Class is used to let wrath phase 2 know they should perform the melee attack.
public class WrathMeleeTrigger : MonoBehaviour
{
    //If the player enters the range we set the flag
    protected void OnTrigger2DEnter(Collider2D other)
    {
        if (other.tag == "Player")
        {
            WrathP2Actor.targetInRange = true;
        }
    }

    //if the player leaves the range we set the flag
    protected void OnTrigger2DExit(Collider2D other)
    {
        if (other.tag == "Player")
        {
            WrathP2Actor.targetInRange = true;
        }
    }
}
