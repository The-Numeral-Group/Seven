using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathChainPull : ActorAbilityFunction<Actor, int>
{
    public float duration;

    // How long player will be stunned when gets hooked.
    public float stunned_duration;

    // The intensity of pull
    public float pullIntensity;

    public GameObject toInstantiateChainList;

    private GameObject player;
    private Actor wrath;
    private GameObject chainList;

    private bool pulled = false;

    public override void Invoke(ref Actor user)
    {
        if (usable)
        {
            isFinished = false;
            InternInvoke(user);
        }
    }
    protected override int InternInvoke(params Actor[] args)
    {
        // Making sure the movementDirection and dragDirection have been resetted.
        args[0].myMovement.MoveActor(Vector2.zero);
        args[0].myMovement.DragActor(Vector2.zero);

        player = GameObject.FindGameObjectsWithTag("Player")?[0];
        wrath = args[0];

        StartCoroutine(wrath.myMovement.LockActorMovement(this.duration));
        StartCoroutine(startChainPulling());
        StartCoroutine(ChainPullFinished());
        return 0;
    }

    private IEnumerator startChainPulling()
    {
        chainList = Instantiate(this.toInstantiateChainList, wrath.transform.position, Quaternion.identity);

        float chainCount = chainList.transform.childCount;

        Vector2 wrathPos = new Vector2(wrath.transform.position.x, wrath.transform.position.y);

        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);

        Vector3 targetDir = playerPos - wrathPos;

        float angle = Mathf.Atan2(targetDir.x, targetDir.y) * Mathf.Rad2Deg;

        angle = angle * -1.0f;

        targetDir.Normalize();

        for (float i = 0; i < chainCount; i++)
        {
            GameObject chain = chainList.transform.GetChild((int)i).gameObject;
            chain.transform.position = new Vector2(wrathPos.x + (1.25f) * i * (targetDir.x), wrathPos.y + (1.25f) * i * (targetDir.y));
            chain.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            chain.SetActive(true);
            yield return new WaitForSeconds(0.05f);
        }

        // Temporary Chain pulling animation
        yield return new WaitForSeconds(0.5f);

        for (float i = chainCount - 1; i >= 0; i--)
        {
            GameObject chain = chainList.transform.GetChild((int)i).gameObject;
            chain.SetActive(false);
            yield return new WaitForSeconds(0.05f);
        }

    }

    private IEnumerator ChainPullFinished()
    {
        yield return new WaitForSeconds(this.duration);
        // Resetting the dragDirection
        wrath.myMovement.DragActor(Vector2.zero);
        // Remove chainList from the scene
        Destroy(chainList);
        pulled = false;
        isFinished = true;
    }

    public IEnumerator testChainFunction()
    {
        if (!pulled)
        {
            pulled = true;
            StartCoroutine(player.GetComponent<Actor>().myMovement.LockActorMovement(this.stunned_duration));

            yield return new WaitForSeconds(this.stunned_duration / 2);

            Vector2 wrathPos = new Vector2(wrath.transform.position.x, wrath.transform.position.y);
            Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);
            Vector2 pullDirection = (wrathPos - playerPos);

            player.GetComponent<Actor>().myMovement.DragActor(pullDirection * pullIntensity);

            yield return new WaitForSeconds(this.stunned_duration / 2);

            player.GetComponent<Actor>().myMovement.DragActor(Vector2.zero);
        }
    }
}
