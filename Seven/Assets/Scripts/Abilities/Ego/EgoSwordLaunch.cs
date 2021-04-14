using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EgoSwordLaunch : ActorAbilityFunction<GameObject, int>
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("The sword prefab to instantiate and launch.")]
    public GameObject swordObj;

    [Tooltip("How far the swords should be from the face anchor.")]
    public Vector2 launchOffset = new Vector2(5f, 5f);

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
                Debug.LogError("EgoSwordLaunch: sword launched with improper target," + 
                    " must be GameObject or Monobehaviour");
            }
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    /*Launches the swords. This ability is defined to launch two swords, one on each side of the
    user's faceAnchor, towards the target. The swords will start facing directly away from the
    face anchor*/
    protected override int InternInvoke(params GameObject[] args)
    {
        //precalculate some positions and rotations
        var rightSide = user.faceAnchor.position + (Vector3)launchOffset;
        var leftSide = user.faceAnchor.position + new Vector3(-launchOffset.x, launchOffset.y, 0f);
        var faceAway = Quaternion.Euler(0f, 0f, 90f);

        //create the literal sword objects
        var swordA = Instantiate(swordObj, rightSide, Quaternion.Inverse(faceAway));
        var swordB = Instantiate(swordObj, leftSide, faceAway);

        //actually launch the swords
        swordA.GetComponent<EgoSwordActor>().Launch(args[0]);
        swordB.GetComponent<EgoSwordActor>().Launch(args[0]);

        return 0;
    }
}
