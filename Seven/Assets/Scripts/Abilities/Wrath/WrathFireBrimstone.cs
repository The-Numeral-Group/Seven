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
        yield return new WaitForSeconds(shake_delay);

        // Do camera shake
        cam.Shake(2.0f, 0.2f);

        for(int i = 0; i < shadowCount; i++)
        {
            // Choose where to spawn

            // Temporary position finder
            float xPos = Random.Range(-25.0f, 25.0f);
            float yPos = Random.Range(-12.0f, 4.0f);

            Instantiate(toInstantiateObject,new Vector3(xPos, yPos, 0.0f), Quaternion.identity);
            yield return new WaitForSeconds(shadowDelay);
        }

        yield return new WaitForSeconds(afterShadowDelay);
        isFinished = true;
    }
}
