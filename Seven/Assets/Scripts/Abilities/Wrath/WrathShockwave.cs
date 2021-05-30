using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathShockwave : ActorAbilityFunction<Actor, int>
{
    // Delay after the fist animation
    public float delayAnim;

    // Delay for each shockwave creation
    public float delayPillarShockwave;

    // Max x and y Scale for large shockwave
    public float maxX, maxY;

    // Delay for large shockwave iteration
    public float delayLargeShockwave;

    // Delay after all the shockwaves were spawned
    public float delayAfterShockwave;

    public string animTrigger;

    public GameObject pillarShockwaveObject;
    public GameObject largeShockwaveObject;

    private float delaySpeedMultiplier;

    public override void Invoke(ref Actor user)
    {
        this.user = user;
        if (usable)
        {
            isFinished = false;
            InternInvoke(user);
        }
    }
    protected override int InternInvoke(params Actor[] args)
    {
        // Making sure the movementDirection and dragDirection have been resetted.
        user.myMovement.MoveActor(Vector2.zero);
        user.myMovement.DragActor(Vector2.zero);

        if (animTrigger.Length != 0)
        {
            user.myAnimationHandler.TrySetTrigger(animTrigger);
        }

        delaySpeedMultiplier = WrathP2Actor.abilitySpeedMultiplier;

        // Chooses type of shockwave
        int shockwaveType = (int)Random.Range(0, 2);

        if(shockwaveType == 0)
        {
            // 4 Shockwave Pillars
            StartCoroutine(startShockwavePillars());
        }
        else
        {
            // 1 Full Room Shockwave
            StartCoroutine(startShockwaveLarge());
        }
        // Temporary calling end function
        //StartCoroutine(CheckIfAnimFinished());

        return 0;
    }

    private IEnumerator startShockwavePillars()
    {
        // Delay after animation
        yield return new WaitForSeconds(delayAnim / delaySpeedMultiplier);

        // Start spawning shockwaves
        List<Vector3> firstCoord = new List<Vector3>() { new Vector3(0.0f, 5.5f, 0.0f), new Vector3(0.0f, 1.5f, -1.0f), new Vector3(0.0f, -2.5f, -2.0f),
                                                         new Vector3(0.0f, -6.5f, -3.0f), new Vector3(0.0f, -10.5f, -4.0f), new Vector3(0.0f, -14.5f, -5.0f)};

        List<Vector3> secondCoord = new List<Vector3>() { new Vector3(7.5f, 8.5f, 0.0f), new Vector3(9.5f, 4.5f, -1.0f), new Vector3(11.5f, 0.5f, -2.0f),
                                                         new Vector3(13.5f, -3.5f, -3.0f), new Vector3(15.5f, -7.5f, -4.0f), new Vector3(17.5f, -11.5f, -5.0f)};

        List<Vector3> thirdCoord = new List<Vector3>() { new Vector3(11.0f, 12.0f, 0.0f), new Vector3(15.0f, 10.0f, -1.0f), new Vector3(19.0f, 8.0f, -2.0f),
                                                         new Vector3(23.0f, 6.0f, -3.0f), new Vector3(27.0f, 4.0f, -4.0f), new Vector3(31.0f, 2.0f, -5.0f)};

        List<Vector3> fourthCoord = new List<Vector3>() { new Vector3(-7.5f, 8.5f, 0.0f), new Vector3(-9.5f, 4.5f, -1.0f), new Vector3(-11.5f, 0.5f, -2.0f),
                                                         new Vector3(-13.5f, -3.5f, -3.0f), new Vector3(-15.5f, -7.5f, -4.0f), new Vector3(-17.5f, -11.5f, -5.0f)};

        List<Vector3> fifthCoord = new List<Vector3>() { new Vector3(-11.0f, 12.0f, 0.0f), new Vector3(-15.0f, 10.0f, -1.0f), new Vector3(-19.0f, 8.0f, -2.0f),
                                                         new Vector3(-23.0f, 6.0f, -3.0f), new Vector3(-27.0f, 4.0f, -4.0f), new Vector3(-31.0f, 2.0f, -5.0f)};

        GameObject shockwaveStorage = new GameObject("WrathP2Shockwaves");

        for (int i = 0; i < 6; i++)
        {
            GameObject firstShockwave = Instantiate(pillarShockwaveObject, firstCoord[i], Quaternion.identity);
            firstShockwave.transform.parent = shockwaveStorage.transform;

            GameObject secondShockwave = Instantiate(pillarShockwaveObject, secondCoord[i], Quaternion.identity);
            secondShockwave.transform.parent = shockwaveStorage.transform;

            GameObject thirdShockwave = Instantiate(pillarShockwaveObject, thirdCoord[i], Quaternion.identity);
            thirdShockwave.transform.parent = shockwaveStorage.transform;

            GameObject fourthShockwave = Instantiate(pillarShockwaveObject, fourthCoord[i], Quaternion.identity);
            fourthShockwave.transform.parent = shockwaveStorage.transform;

            GameObject fifthShockwave = Instantiate(pillarShockwaveObject, fifthCoord[i], Quaternion.identity);
            fifthShockwave.transform.parent = shockwaveStorage.transform;

            yield return new WaitForSeconds(delayPillarShockwave / delaySpeedMultiplier);
        }

        yield return new WaitForSeconds(delayAfterShockwave / delaySpeedMultiplier);
        isFinished = true;
    }

    private IEnumerator startShockwaveLarge()
    {
        // Delay after animation
        yield return new WaitForSeconds(delayAnim / delaySpeedMultiplier);

        Vector3 pos = new Vector3(0f, 2.0f, -1.0f);
        GameObject shockwave = Instantiate(largeShockwaveObject, pos, Quaternion.identity);

        while (shockwave.transform.localScale.x <= maxX && shockwave.transform.localScale.y <= maxY)
        {
            var currScale = shockwave.transform.localScale;
            currScale.x += 0.1f;
            currScale.y += 0.1f;
            shockwave.transform.localScale = currScale;
            yield return new WaitForSeconds(delayLargeShockwave / delaySpeedMultiplier);
        }

        yield return new WaitForSeconds(delayAfterShockwave / delaySpeedMultiplier);
        isFinished = true;
        Destroy(shockwave);
    }
}

