using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Doc: https://docs.google.com/document/d/1OXQWXkKULq-NAye80N1zh7xA3UmZU50Jc8pDJfWaitc/edit
public class GhostKnightIdleActor : Actor
{

    Actor ghostKnight;
    Actor player;

    GameObject ghostKnightEffector;

    PointEffector2D pointEffector;
    
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        ghostKnight = this.gameObject.GetComponent<GhostKnightIdleActor>();
        ghostKnightEffector = GameObject.FindGameObjectWithTag("Boss Effector");

        if(ghostKnightEffector == null)
        {
            Debug.Log("GhostKnightIdleActor: Cannot find the Effector object!");
        }

        var playerObject = GameObject.FindGameObjectsWithTag("Player")?[0];

        if (playerObject == null)
        {
            Debug.LogWarning("GhostKnightActor: Ghost Knight can't find the player!");
        }
        else
        {
            player = playerObject.GetComponent<Actor>();
        }

        player.myHealth.SetVulnerable(false, -1);
        ghostKnight.myHealth.SetVulnerable(false, -1);
        
        ghostKnightEffector.SetActive(false);

    }

}
