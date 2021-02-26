using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostKnightIdleActor : Actor
{
    public float introDelay = 2f;

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

        player.myHealth.vulnerable = false;
        ghostKnight.myHealth.vulnerable = false;

        ghostKnightEffector.SetActive(false);

        MenuManager.DIALOGUE_MENU.StartDialogue(this.gameObject);
    }

    /*private IEnumerator introDelayStart()
    {
        yield return new WaitForSeconds(this.introDelay);
        this.attackPhase = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (attackPhase && !changingPhase)
        {
            changingPhase = true;

            ghostKnightEffector.SetActive(true);

            System.Tuple<Actor, System.Action<Actor>> ghostKnightFight =
                new System.Tuple<Actor, System.Action<Actor>>(ghostKnight, null);
            gameObject.SendMessage("NextPhase", ghostKnightFight);
        }
    }*/
}
