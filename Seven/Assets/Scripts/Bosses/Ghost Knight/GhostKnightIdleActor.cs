using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostKnightIdleActor : Actor
{
    public float introDelay = 2f;

    private bool attackPhase = false;

    private bool changingPhase = false;

    Actor ghostKnight;
    
    
    // Start is called before the first frame update
    new void Start()
    {
        ghostKnight = this.gameObject.GetComponent<GhostKnightIdleActor>();
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
            System.Tuple<Actor, System.Action<Actor>> ghostKnightFight =
                new System.Tuple<Actor, System.Action<Actor>>(ghostKnight, null);
            gameObject.SendMessage("NextPhase", ghostKnightFight);
        }
    }
}
