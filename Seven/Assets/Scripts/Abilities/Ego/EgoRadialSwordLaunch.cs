using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EgoRadialSwordLaunch : ActorAbilityCoroutine<GameObject>
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("The sword prefab to instantiate and launch.")]
    public GameObject swordObj;

    [Tooltip("How far the swords should be from the target.")]
    public float targetOffset = 5f;

    [Tooltip("How far the swords should be from each other.")]
    public float radialOffset = 5f;

    [Tooltip("How many seconds should be between each sword launch.")]
    public float launchTimeOffset = 2f;

    //METHODS--------------------------------------------------------------------------------------
    /*Activates the ability with no arguments. In this case, it will default the target to
    the player*/
    public override void Invoke(ref Actor user)
    {
        this.user = user;
        if(usable)
        {
            isFinished = false;
            InternInvoke(GameObject.FindWithTag("Player"));
            StartCoroutine(coolDown(cooldownPeriod));
        }
        
    }

    /*Same as the above method, but with a provided vector position*/
    public override void Invoke(ref Actor user, params object[] args)
    {
        this.user = user;
        //by default, Invoke just does InternInvoke with the provided arguments
        //it's also just implicitly convert the args and give it to InternInvoke
        if(usable)
        {
            isFinished = false;

            //make sure the argument is a gameObject of some sort
            if(args[0] is GameObject)
            {
                InternInvoke((args[0] as GameObject));
            }
            else if(args[0] is MonoBehaviour)
            {
                InternInvoke((args[0] as MonoBehaviour).gameObject);
            }
            else
            {
                Debug.LogError("EgoRadialSwordLaunch: sword launched with improper target," + 
                    " must be GameObject or Monobehaviour");
            }
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    /*The behaviour of the swords. Place them over the target, wait a little bit, and then
    launch them all as normal.*/
    protected override IEnumerator InternCoroutine(params GameObject[] args)
    {
        //Step 0: make a queue to easily manage the swords
        Queue<GameObject> swordQueue = new Queue<GameObject>();

        //Step 0.1: Save target
        GameObject target = args[0];

        //Step 1: Generate a bunch of swords in different offset positions and add them to the
        //queue
        for(float rOffset = -radialOffset; rOffset <= radialOffset; rOffset += radialOffset)
        {
            //Step 1.1: calculate sword position
            var swordPos = new Vector3
            (
                target.transform.position.x + rOffset,
                target.transform.position.y + targetOffset,
                target.transform.position.z + rOffset
            );

            //Step 1.2: calculate sword direction
            //needs to be negative due to the relative orientation of the prefab itself
            var swordDir = (target.transform.position - swordPos).normalized;

            //Step 1.25: create the sword
            var sword = Instantiate
            (
                swordObj,
                swordPos,
                Quaternion.Euler(swordDir.x, swordDir.y, -swordDir.z)
            );

            //Step 1.3: Create the sword and enqueue it
            swordQueue.Enqueue(sword);

            //Step 1.4: Tell it to delayLaunch
            EgoSwordActor swordActor;
            if(sword.TryGetComponent(out swordActor))
            {
                swordActor.DelayFollowLaunch(
                    target, 
                    new Vector3(rOffset, targetOffset, 0f), 
                    launchTimeOffset * ((rOffset / radialOffset) + 1f)
                );
            }
            else
            {
                Debug.LogError("EgoRadialSwordLaunch: swords are missing their EgoSwordActor" + 
                    " components. Swords cannot be launched");
                Destroy(sword);
            }


            //That makes one sword. This loop will run 3 times
        }

        //just wait out launch times then return
        yield return new WaitForSeconds(launchTimeOffset * 3);
        yield break;

        //Step 2: Launch the swords
        while(swordQueue.Count != 0)
        {
            //Step 2.2: wait a little bit
            yield return new WaitForSeconds(launchTimeOffset);

            //Step 2.3: get a sword to launch
            var sword = swordQueue.Dequeue();

            //Step 2.4: de-parent it
            sword.transform.parent = null;

            //Step 2.5: launch it at the target
            EgoSwordActor swordActor;
            if(sword.TryGetComponent(out swordActor))
            {
                swordActor.Launch(target);
            }
            else
            {
                Debug.LogError("EgoRadialSwordLaunch: swords are missing their EgoSwordActor" + 
                    " components. Swords cannot be launched");
                Destroy(sword);
            }

            //It is an error if swordObj does not have an EgoSwordActor
        }

        //By the time we get here, the method is done!
    }
}
