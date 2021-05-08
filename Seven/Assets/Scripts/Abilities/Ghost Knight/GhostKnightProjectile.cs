using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Doc: https://docs.google.com/document/d/1pHACk_wBPStez-ZhAZ1oEYqzvvStwrXwg9e3OY0W8ZQ/edit
public class GhostKnightProjectile : ActorAbilityFunction<Actor, int>
{
    //Where ghost knight will move towards before initiating the attack.
    public Vector2 centerPos;
    //The projectile that ghost knight will spawn. Must have an ActorMovement component.
    public GameObject toInstantiateProjectile;
    //Time ghost knight will take to move to center.
    public float travelDuration;
    //Time it will take to spawn the projectiles.
    public float projectileSpawnTime;
    //Number of projectiles to spawn.
    public int numProjectiles = 4;
    //Offset of how far projectiles spawn from the ghost knight
    public float offsetValue = 1f;

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
        if (this.travelDuration <= 0f || this.projectileSpawnTime <= 0f)
        {
            Debug.Log("GhostKnightPhaseChange: duration/projectileDelay must be greater than 0");
            this.travelDuration = 10f;
            this.projectileSpawnTime = 2f;
        }
        StartCoroutine(args[0].myMovement.LockActorMovementOnly(this.travelDuration + this.projectileSpawnTime));
        StartCoroutine(MoveToPoint(args[0]));

        return 0;
    }

    // Source: GluttonyProjectileP1.cs
    // This coroutine will move the user to the position dictates by the memebr variable centerPos.
    // The speed of the movement is determined by the duration of this entire ability subtracted from the time spend managing the projectiles.
    // Therefore the speed the actor moves is variable and will either be slower or faster depening on distance from centerPos; 
    private IEnumerator MoveToPoint(Actor user)
    {
        Vector2 direction = this.centerPos - new Vector2(user.gameObject.transform.position.x, user.gameObject.transform.position.y);
        /*direction.Normalize();
        /*
        float distance = Vector2.Distance(this.centerPos, user.gameObject.transform.position);
        float speed = distance / (this.travelDuration);*/
        //Debug.Log(direction);
        user.myMovement.DragActor(direction);
        yield return new WaitForSeconds(this.travelDuration);
        user.myMovement.DragActor(Vector2.zero);
        StartCoroutine(SpawnProjectiles(user));
    }

    private IEnumerator SpawnProjectiles(Actor user)
    {
        user.mySoundManager.PlaySound("ProjectileAppears");

        List<GameObject> projectiles = new List<GameObject>();

        float[,] offset = new float[,] { { -7.0f, 9.0f }, { -3.0f, 9.0f }, 
                                         { 3.0f, 9.0f }, { 7.0f, 9.0f } };

        for (int i = 0; i < numProjectiles; i++)
        {
            Vector2 projPos = new Vector2(offset[i, 0], offset[i, 1]);
            GameObject ghostKnightProjectile = Instantiate(this.toInstantiateProjectile, projPos, Quaternion.identity);
            projectiles.Add(ghostKnightProjectile);
            yield return new WaitForSeconds(this.projectileSpawnTime/5);
        }
        for (int i = 0; i < numProjectiles; i++)
        {
            projectiles[i].GetComponent<GhostKnightProjectileMovement>().setProjMove(true);
        }
        yield return new WaitForSeconds(this.projectileSpawnTime / 5);
        isFinished = true;
    }
}
