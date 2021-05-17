using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathSludge : ActorAbilityFunction<GameObject, int>
{
    [Tooltip("The numer of projectile pairs this ability should launch")]
    public int pairCount = 2;

    [Header("Projectile Settings")]

    [Tooltip("The prefab of the Projectile.")]
    public GameObject projectile;

    [Tooltip("The prefab of the Marker.")]
    public GameObject marker;

    //the target will come from Invoke

    [Tooltip("How far ahead of the target the leading markers should be.")]
    public float leadMarkerDistance = 2f;

    [Tooltip("How long the markers should follow before they stop moving.")]
    public float initialWaitTime = 3f;

    [Tooltip("How long it should take for a projectile to reach its target.")]
    public float shotFlightTime = 2f;

    //the single-shot version of the ability
    private WrathSludgeSingle single;

    private IEnumerator MovementLockroutine;

    // Start is called before the first frame update
    void Start()
    {
        single = this.gameObject.AddComponent<WrathSludgeSingle>();
        single.Init(this);
    }

    /*Activates the ability with no arguments. In this case, it will default the target to
    the player*/
    public override void Invoke(ref Actor user)
    {
        this.user = user;
        if (usable)
        {
            isFinished = false;
            InternInvoke(GameObject.FindWithTag("Player"));
        }

    }

    /*Same as the above method, but with a provided vector position*/
    public override void Invoke(ref Actor user, params object[] args)
    {
        this.user = user;
        //by default, Invoke just does InternInvoke with the provided arguments
        //it's also just implicitly convert the args and give it to InternInvoke
        if (usable)
        {
            isFinished = false;

            //make sure the argument is a gameObject of some sort
            if (args[0] is GameObject)
            {
                InternInvoke((args[0] as GameObject));
            }
            else if (args[0] is MonoBehaviour)
            {
                InternInvoke((args[0] as MonoBehaviour).gameObject);
            }
            else
            {
                Debug.LogError("wrathFireLaunch: firewall launched with improper target," +
                    " must be GameObject or Monobehaviour");
            }
        }
    }

    //Activates an ActorAbility. Just wraps a coroutine.
    protected override int InternInvoke(params GameObject[] args)
    {
        MovementLockroutine = this.user.myMovement.LockActorMovement(Mathf.Infinity); // Lock Wrath's movement inf, unlock when done.
        StartCoroutine(SludgeInvokation(args[0]));
        StartCoroutine(MovementLockroutine);
        return 1;
    }

    IEnumerator SludgeInvokation(GameObject target)
    {
        //uhhh do it X many times idk
        for (int i = pairCount; i > 0; --i)
        {
            yield return new WaitUntil(() => single.getUsable());
            Debug.Log($"WrathSludege: attack cycle {i}");
            single.target = target.transform;
            single.Invoke(ref user, target);
            yield return new WaitUntil(() => single.getIsFinished());
        }
        // Teleport Wrath to Player.
        this.user.transform.position = target.transform.position;
        SludgeFinished();
    }

    private void SludgeFinished()
    {
        this.user.myMovement.DragActor(Vector2.zero);
        StopCoroutine(MovementLockroutine); // Unlock wrath's movement.
        isFinished = true;
    }
}

internal class WrathSludgeSingle : ProjectileAbility
{
    //All of thes variables carry the same meaning as they do in WrathSludge
    public Transform target { get; set; }

    private GameObject marker;

    private float leadMarkerDistance;

    private float initialWaitTime;

    private float shotFlightTime;

    //recieves relevant initialization data
    public void Init(WrathSludge wrap)
    {
        this.projectile = wrap.projectile;
        this.marker = wrap.marker;
        this.leadMarkerDistance = wrap.leadMarkerDistance;
        this.initialWaitTime = wrap.initialWaitTime;
        this.shotFlightTime = wrap.shotFlightTime;
    }

    /*Launch the projectile. The anticipated argument is the gameObject being shot at. The 
    gameObject may-or-may-not have an ActorHealth component.*/
    protected override int InternInvoke(params Vector2[] args)
    {
        //deactivate projObj, we actually don't need it yet
        projObj.SetActive(false);

        //Start the long-term dup shooting
        StartCoroutine(ProjectileBehaviour());

        return 0;
    }

    /*The actual behaviour of the projectile sequence. Uses Coroutines for the literal timing of
    shots and markers.*/
    IEnumerator ProjectileBehaviour()
    {
        //Step -1: Start the animation
        //Going from Idle to prethrow to throwstance

        // MISSING ANIMATION --------------------------------------------------------
        //var animDelegate = this.user.myAnimationHandler.TryFlaggedSetTrigger("Wrath_Throw");

        //Step 0: check to see if there is a FaceAnchor for the target
        var targetFace = target.Find("FaceAnchor");

        //Step 1: Make a marker object where the player is standing
        //Instantiate Projectile makes this kinda easy
        var markObjA = this.InstantiateProjectile(marker, target, Vector2.zero);

        //Step 2: Actually make two
        //This one leads in front of the player though
        var markObjB = this.InstantiateProjectile(marker, target, target.up * leadMarkerDistance);

        //Step 3: Give them both marker components
        //and Step 4: Tell them to follow the player in their current relative positions
        markObjA.AddComponent<WrathSludgeMarker>().Follow(target, 0f, initialWaitTime);

        //markObjB will follow based off of faceAnchor if one exists
        markObjB.AddComponent<WrathSludgeMarker>()
            .Follow(target, leadMarkerDistance, initialWaitTime + shotFlightTime, targetFace);

        //Step 5: wait some seconds
        yield return new WaitForSeconds(initialWaitTime);
        //Step 5.5: and for the animation to finish
        //this should wait until the animator exits the prethrow state

        // MISSING ANIM --------------------------------------------------------
        //yield return new WaitWhile(animDelegate);

        //Step 6: Calcualte how fast the projectile needs to go to arrive in shotFlightTime seconds
        //d = rt: speed is distance to marker divided by shotFlightTime
        //Mathf.Abs because we want absolute speed (the projectile will handle direction)
        var newSpeed =
            Mathf.Abs(Vector3.Distance(markObjA.transform.position, projObj.transform.position))
                / shotFlightTime;
        projObj.GetComponent<ActorMovement>().speed = newSpeed;

        //Step: 6.25 animate the next part of the throw
        //Going from throwstance to throw to idle

        // MISSING ANIM --------------------------------------------------------
        //animDelegate = this.user.myAnimationHandler.TryFlaggedSetTrigger("Wrath_Throw");

        //Step 6.5: Actually launch the projectile at the directly under marker now
        projObj.SetActive(true);
        projObj.GetComponent<FilterProjectile>()
            .Launch(markObjA.transform.position, LAUNCH_MODE.POINT, target.gameObject);

        //Step 6.75: wait for the previous anim to finish
        //this should wait until the animator exits the throw state

        // MISSING ANIM --------------------------------------------------------
        //yield return new WaitWhile(animDelegate);

        //Step 6.875: animate the next part of the throw
        //Going from Idle to prethrow to throwstance

        // MISSING ANIM --------------------------------------------------------
        //animDelegate = this.user.myAnimationHandler.TryFlaggedSetTrigger("Wrath_Throw");

        //Step 7: Wait a little bit again...
        yield return new WaitForSeconds(shotFlightTime);

        //Step 7.5: And for the animation to finish
        //this should wait until the animator exits the prethrow state

        // MISSING ANIM --------------------------------------------------------
        //yield return new WaitWhile(animDelegate);


        //Step 8: Destroy the marker and the projectile, if they still exist
        Destroy(markObjA);
        //We need to double check here in case proj obj was destroyed on impact
        if (projObj)
        {
            Destroy(projObj);
        }

        //Step 9: Now we need a brand new proj obj
        projObj = InstantiateProjectile(projectile, user.faceAnchor, projectileScale);

        //Step 10: Gotta switch this thing's speed too
        //d = rt: speed is distance to marker divided by shotFlightTime
        //Mathf.Abs because we want absolute speed (the projectile will handle direction)
        newSpeed =
            Mathf.Abs(Vector3.Distance(markObjB.transform.position, projObj.transform.position))
                / shotFlightTime;
        projObj.GetComponent<ActorMovement>().speed = newSpeed;

        //Step: 10.25 animate the next part of the throw
        //we don't need to wait for this animation to end

        // MISSING ANIM --------------------------------------------------------
        //this.user.myAnimationHandler.TrySetTrigger("Wrath_Throw");

        //Step 10.5: And we're gonna launch it too
        projObj.GetComponent<FilterProjectile>()
            .Launch(markObjB.transform.position, LAUNCH_MODE.POINT, target.gameObject);

        //Step 11: Wait again...
        yield return new WaitForSeconds(shotFlightTime);

        //Step 12: Destroy the marker and the projectile, if they still exist
        Destroy(markObjB);
        //We need to double check here in case proj obj was destroyed on impact
        if (projObj)
        {
            Destroy(projObj);
        }

        //Step 13: Cooldown
        StartCoroutine(coolDown(cooldownPeriod));

        //Step 14: And now we're done! Whew!
        isFinished = true;
    }
}

internal class WrathSludgeMarker : MonoBehaviour
{
    //This is a simple wrapper for the coroutine
    public void Follow(Transform target, float offset, float duration, Transform faceAnchor = null)
    {
        StartCoroutine(InternFollow(target, offset, duration, faceAnchor));
    }

    /*The following period. Will immediately snap this marker to the target's position,
    modified by the offset, as long as duration holds. Because there are NO phyiscal
    interations with these markers, it's okay to snap their positions directly.*/
    IEnumerator InternFollow(Transform target, float offset,
        float duration, Transform faceAnchor)
    {
        float timer = 0f;

        //Dowhile will make the marker move before time is checked, so it always gets once
        //chance to update its position
        do
        {
            Vector3 directionToTarget;
            //if the target has a faceAnchor, judge the direction they're facing from that
            if (faceAnchor)
            {
                Vector3 pos = target.position;
                directionToTarget = (faceAnchor.position - pos).normalized;
                directionToTarget *= offset;
            }
            else
            {
                directionToTarget = target.up * offset;
            }
            this.gameObject.transform.position = target.position + directionToTarget;

            yield return null;

            timer += Time.deltaTime;
        }
        while (timer < duration);
    }
}
