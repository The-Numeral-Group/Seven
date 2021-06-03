using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathChainPull : ActorAbilityFunction<Actor, int>
{
    // Where wrath will move towards before initiating the chain pull.
    public Vector2 centerPos;

    // The duration of the ability. (How long the wrath's movement will be locked for)
    public float duration;

    // The delay after animation gets played
    public float animDelay;
    
    // The duration that wrath will take to move to the centerPos.
    public float travelDuration;

    // How long the player will be pulled
    public float pullDuration;
   
    // How long player will be stunned when gets hooked.
    public float stunnedDuration;


    // The number of pulls that wrath will attempt before moving on.
    public float pullAttemptNumber;

    // The intensity of pull
    public float pullIntensity;

    public GameObject toInstantiateChainList;

    private IEnumerator MovementLockroutine;

    private GameObject player;
    private Actor wrath;
    private GameObject chainList;

    private bool pulled;
    private int pulledNumber;

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

        pulledNumber = 0;
        pulled = false;

        MovementLockroutine = wrath.myMovement.LockActorMovement(Mathf.Infinity);
        StartCoroutine(MovementLockroutine);
        StartCoroutine(MoveToPoint());
        return 0;
    }
    
    // Wrath moving to centerPos. (Under the fountain)
    private IEnumerator MoveToPoint()
    {
        Vector2 direction = this.centerPos - new Vector2(wrath.gameObject.transform.position.x, wrath.gameObject.transform.position.y);
        direction.Normalize();
        float distance = Vector2.Distance(this.centerPos, wrath.gameObject.transform.position);
        float speed = distance / (this.travelDuration);
        wrath.myMovement.DragActor(direction * speed);
        yield return new WaitForSeconds(this.travelDuration);
        wrath.myMovement.DragActor(Vector2.zero);
        StartCoroutine(startChainPulling());
    }

    private IEnumerator startChainPulling()
    {
        for (int i = 0; i < pullAttemptNumber; i++)
        {
            if(!pulled) // Player has not been pulled yet, keep throwing chain.
            {
                wrath.myAnimationHandler.TrySetTrigger("Wrath_Chain");
                yield return new WaitForSeconds(animDelay);

                pulledNumber++;
                chainList = Instantiate(this.toInstantiateChainList, wrath.transform.position, Quaternion.identity);
                StartCoroutine(spawnChainPulling());
                yield return new WaitForSeconds(1.0f); // Each chain lasts for 1 second. 
                Destroy(chainList);
            }
            else // Player has been pulled, end the ability early. 
            {
                StartCoroutine(ChainPulledFinishedEarly());
            }
        }
        if(pulledNumber == 4)
        {
            ChainPullFinished();
        }

    }

    private IEnumerator spawnChainPulling()
    {
        Vector2 wrathPos = new Vector2(wrath.transform.position.x, wrath.transform.position.y);

        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);

        Vector3 targetDir = playerPos - wrathPos;

        // Get chain count based on player's position.
        int chainCount = (int)Vector2.Distance(playerPos, wrathPos);
        // Each chain lasts for 1 second, and delay between each chain is 0.03 seconds. 
        // Therefore, the maximum chainCount can be 33. (0.03 * 33 = 0.99) Cannot go over 1. 
        // This number can be changed. 
        chainCount = Mathf.Min(chainCount, 33); 

        float angle = Mathf.Atan2(targetDir.x, targetDir.y) * Mathf.Rad2Deg;

        angle = angle * -1.0f;

        targetDir.Normalize();

        for (int i = 0; i < chainCount; i++)
        {
            if (i == chainCount-1) // Create chain shackle 
            {
                GameObject chainShackle = Instantiate(chainList.transform.GetChild(0).gameObject, wrath.transform.position, Quaternion.identity);
                chainShackle.transform.parent = chainList.transform;
                chainShackle.transform.position = new Vector2(wrathPos.x + (1.25f) * i * (targetDir.x), wrathPos.y + (1.25f) * i * (targetDir.y));
                chainShackle.transform.rotation = Quaternion.Euler(0f, 0f, angle);
                chainShackle.SetActive(true);
            }
            else // Create chain body
            {
                GameObject chainBody = Instantiate(chainList.transform.GetChild(1).gameObject, wrath.transform.position, Quaternion.identity);
                chainBody.transform.parent = chainList.transform;
                chainBody.transform.position = new Vector2(wrathPos.x + (1.25f) * i * (targetDir.x), wrathPos.y + (1.25f) * i * (targetDir.y));
                chainBody.transform.rotation = Quaternion.Euler(0f, 0f, angle);
                chainBody.SetActive(true);
            }
            // How fast each chain gets created. This delay can be changed.
            yield return new WaitForSeconds(0.03f);
        }

    }

    private IEnumerator ChainPulledFinishedEarly()
    {
        StopCoroutine(MovementLockroutine); // Stop the original movement Lock function
        StartCoroutine(wrath.myMovement.LockActorMovement(0.5f));
        yield return new WaitForSeconds(0.5f); // In case if you want to add any delay after chain pull. 
        wrath.myMovement.DragActor(Vector2.zero);
        pulled = false;
        isFinished = true;
    }

    private void ChainPullFinished()
    {
        // Resetting the dragDirection
        wrath.myMovement.DragActor(Vector2.zero);
        pulled = false;
        isFinished = true;
    }

    // NOTE: There is a bug where if player dashes when gets collided with chain, player teleports instead of getting pulled. 
    public IEnumerator onChainCollision()
    {
        if (!pulled)
        {
            pulled = true;
            // Lock player's movement
            StartCoroutine(stunPlayer());

            yield return new WaitForSeconds(this.pullDuration / 2);

            Vector2 wrathPos = new Vector2(wrath.transform.position.x, wrath.transform.position.y);
            Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);
            Vector2 pullDirection = (wrathPos - playerPos);

            player.GetComponent<Actor>().myMovement.DragActor(pullDirection * pullIntensity);

            yield return new WaitForSeconds(this.pullDuration / 2);

            player.GetComponent<Actor>().myMovement.DragActor(Vector2.zero);

        }
    }

    private IEnumerator stunPlayer()
    {
        // Lock player's movement
        StartCoroutine(player.GetComponent<Actor>().myMovement.LockActorMovement(this.stunnedDuration));

        // Turn off player's dodge ability.
        player.GetComponent<PlayerAbilityInitiator>().canDodge = false;

        yield return new WaitForSeconds(this.stunnedDuration);

        // Turn on player's dodge ability.
        player.GetComponent<PlayerAbilityInitiator>().canDodge = true;

    }
}
