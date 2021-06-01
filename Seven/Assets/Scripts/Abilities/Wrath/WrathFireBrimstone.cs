using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathFireBrimstone : ActorAbilityFunction<Actor, int>
{
    // Number of shadows that will get spawned.
    public int shadowCount;

    // Delay between each shadow spawn
    public float shadowDelay;

    // Delay after all shadows get spawned
    public float afterShadowDelay;

    public string animTrigger;

    public GameObject toInstantiateObject;

    public GameObject areaToChoose;

    private float delaySpeedMultiplier;


    // Delay time before camera shake
    private float shake_delay = 1.0f;

    // Camera object for Camera Shake
    private BaseCamera cam;
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

        var camObjects = FindObjectsOfType<BaseCamera>();
        if (camObjects.Length > 0)
        {
            cam = camObjects[0];
        }

        delaySpeedMultiplier = WrathP2Actor.abilitySpeedMultiplier;

        // Play fist animation
        if (animTrigger.Length != 0)
        {
            user.myAnimationHandler.TrySetTrigger(animTrigger);
        }

        StartCoroutine(spawnShadow());
        return 0;
    }

    private IEnumerator spawnShadow()
    {
        // Delay before camera shake
        yield return new WaitForSeconds(shake_delay / delaySpeedMultiplier);

        // Play fist sound
        user.mySoundManager.PlaySound("fist");

        // Do camera shake
        cam.Shake(2.0f, 0.2f);

        //Debug.Log(areaToChoose.GetComponent<PolygonCollider2D>().bounds.min);

        for(int i = 0; i < shadowCount; i++)
        {
            // Choose where to spawn
            var bounds = areaToChoose.GetComponent<Collider2D>().bounds;
            var center = bounds.center;

            float xPos = 0;
            float yPos = 0;

            do
            {
                xPos = Random.Range(center.x - bounds.extents.x, center.x + bounds.extents.x);
                yPos = Random.Range(center.y - bounds.extents.y, center.y + bounds.extents.y);
            } while (!areaToChoose.GetComponent<Collider2D>().OverlapPoint(new Vector2(xPos, yPos)));

            Instantiate(toInstantiateObject,new Vector3(xPos, yPos, 0.0f), Quaternion.identity);
            yield return new WaitForSeconds(shadowDelay / delaySpeedMultiplier);
        }

        yield return new WaitForSeconds(afterShadowDelay / delaySpeedMultiplier);
        isFinished = true;
    }
}
