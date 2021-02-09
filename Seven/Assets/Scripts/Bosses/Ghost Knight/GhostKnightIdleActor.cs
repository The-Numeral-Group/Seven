using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostKnightIdleActor : Actor
{
    public float introDelay = 2f;

    private bool attackPhase = false;

    private bool changingPhase = false;

    Actor ghostKnight;

    GameObject ghostKnightEffector;
    PointEffector2D pointEffector;
    
    
    // Start is called before the first frame update
    new void Start()
    {
        ghostKnight = this.gameObject.GetComponent<GhostKnightIdleActor>();
        ghostKnightEffector = GameObject.FindGameObjectWithTag("Boss Effector");

        if(ghostKnightEffector == null)
        {
            Debug.Log("GhostKnightIdleActor: Cannot find the Effector object!");
        }

        ghostKnightEffector.SetActive(false);
        StartCoroutine(introDelayStart());
    }

    private IEnumerator introDelayStart()
    {
        yield return new WaitForSeconds(this.introDelay);
        this.attackPhase = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(attackPhase && !changingPhase)
        {
            changingPhase = true;

            ghostKnightEffector.SetActive(true);

            System.Tuple<Actor, System.Action<Actor>> ghostKnightFight =
                new System.Tuple<Actor, System.Action<Actor>>(ghostKnight, null);
            gameObject.SendMessage("NextPhase", ghostKnightFight);
        }
    }
}
