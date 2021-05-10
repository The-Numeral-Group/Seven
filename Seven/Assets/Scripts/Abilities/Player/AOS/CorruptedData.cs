using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptedData : ActorAbilityFunction<Actor, int>
{
    [Range(0f, 10)]
    public float projSpeed = 6f;
    [Range(0f, 2 * Mathf.PI)]
    public float projectileSpread = Mathf.PI/4f;
    [Range(0f, 5f)]
    public int numProjectiles = 3;
    public GameObject projectileToInstantiate;

    void Start()
    {
        if (projectileToInstantiate == null)
        {
            Debug.LogWarning("CorruptedDataAbility: No prefab set, disabling ability");
            this.enabled = false;
        }
    }
    public override void Invoke(ref Actor user)
    {
        this.user = user;
        //by default, Invoke just does InternInvoke with no arguments
        if(usable && this.isFinished && user.myHealth.currentHealth > 1)
        {
            isFinished = false;
            InternInvoke(new Actor[0]);
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    public override void Invoke(ref Actor user, params object[] args)
    {
        Invoke(ref user);
    }

    protected override int InternInvoke(params Actor[] args)
    {
        Vector2 originalDirection = user.faceAnchor.localPosition;
        int directionMultiplier = 1;
        user.myHealth.takeDamage(1f, false, true);
        for (int i = 0; i < numProjectiles; i++)
        {
            if (i % 2 == 1)
            {
                directionMultiplier = -1;
            }
            else
            {
                directionMultiplier = 1;
            }
            float dtheta = projectileSpread * directionMultiplier *  ((i+1)/2);
            Vector2 newDirection = new Vector2(originalDirection.x * Mathf.Cos(dtheta) - originalDirection.y * Mathf.Sin(dtheta),
                            originalDirection.x * Mathf.Sin(dtheta) + originalDirection.y * Mathf.Cos(dtheta));
            GameObject projectile = Instantiate(projectileToInstantiate, this.transform.position, Quaternion.identity);
            projectile.GetComponent<ActorMovement>().DragActor(newDirection * projSpeed);
        }
        this.isFinished = true;
        return 0;
    }
}
