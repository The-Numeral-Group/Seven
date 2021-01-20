using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostKnightSlash : ActorAbilityFunction<Actor, int>
{
    //The Slash Object that ghost knight will spawn.
    public GameObject vSlash;
    public GameObject hSlash;
    //How long this entire process should take.
    public float duration = 2f;
    //How long the attack animation lasts;
    public float animationDuration = 1f;

    public override void Invoke(ref Actor user)
    {
        if (usable)
        {
            isFinished = false;
            InternInvoke(user);
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    protected override int InternInvoke(params Actor[] args)
    {
        if (this.duration <= 0f)
        {
            Debug.Log("GhostKnightPhaseChange: duration must be greater than 0");
            this.duration = 2f;
        }
        StartCoroutine(args[0].myMovement.LockActorMovement(duration));
        int whichAtt = (int)Random.Range(1, 3);
        if (whichAtt == 1)
        {
            PerformVSlash(args[0]);
        }
        else
        {
            PerformHSlash(args[0]);
        }
        return 0;
    }
    
    /* Right now, Slash spawns an object (red rectangle) to indicate the slash range
       For the final version, Slash won't spawn any object. Instead, animation will
       do the job for revealing the slash range. You can insert collider in animation 
       and that will do the job for hurting the player */
    private void PerformVSlash(Actor user)
    {
        Vector2 userPos = user.gameObject.transform.position;
        //Debug.Log(user.myMovement.movementDirection);
        GameObject ghostKnightSlash = Instantiate(this.vSlash, userPos, Quaternion.identity);
        StartCoroutine(DestroySlashObject(ghostKnightSlash));
    }
    private void PerformHSlash(Actor user)
    {
        Vector2 userPos = user.gameObject.transform.position;
        //Debug.Log(user.myMovement.movementDirection);
        GameObject ghostKnightSlash = Instantiate(this.hSlash, userPos, Quaternion.identity);
        StartCoroutine(DestroySlashObject(ghostKnightSlash));
    }
    private IEnumerator DestroySlashObject(GameObject obj)
    {
        yield return new WaitForSeconds(this.animationDuration);
        Destroy(obj);
        isFinished = true;
    }
}
