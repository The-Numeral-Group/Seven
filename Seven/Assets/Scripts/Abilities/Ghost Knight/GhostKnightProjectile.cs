﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostKnightProjectile : ActorAbilityFunction<Actor, int>
{
    //Where ghost knight will move towards before initiating the attack.
    public Vector2 centerPos = Vector2.zero;
    //The projectile that ghost knight will spawn. Must have an ActorMovement component.
    public GameObject toInstantiateProjectile;
    //How long this entire process should take.
    public float duration = 2f;
    //How long the projectiles will last for. Must be greated thatn 0.
    public float projectileDuration = 5f;
    //Time it will take to spawn the projectiles.
    public float projectileSpawnTime = 1f;
    //Number of projectiles to spawn.
    public int numProjectiles = 4;
    //Offset of how far projectiles spawn from the ghost knight
    public float offsetValue = 1f;

    private List<GameObject> projectileManager = new List<GameObject>();

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
        if (this.duration <= 0f || this.projectileSpawnTime <= 0f)
        {
            Debug.Log("GhostKnightPhaseChange: duration/projectileDelay must be greater than 0");
            this.duration = 10f;
            this.projectileSpawnTime = 2f;
        }

        StartCoroutine(args[0].myMovement.LockActorMovement(this.duration));
        StartCoroutine(MoveToCenter(args[0]));
        return 0;
    }

    // Source: GluttonyProjectileP1.cs
    // This coroutine will move the user to the position dictates by the memebr variable centerPos.
    // The speed of the movement is determined by the duration of this entire ability subtracted from the time spend managing the projectiles.
    // Therefore the speed the actor moves is variable and will either be slower or faster depening on distance from centerPos; 
    private IEnumerator MoveToCenter(Actor user)
    {
        Vector2 direction = this.centerPos - new Vector2(user.gameObject.transform.position.x, user.gameObject.transform.position.y);
        direction.Normalize();
        float distance = Vector2.Distance(this.centerPos, user.gameObject.transform.position);
        float speed = distance / (this.duration - this.projectileSpawnTime);
        user.myMovement.DragActor(direction * speed);
        yield return new WaitForSeconds(this.duration - this.projectileSpawnTime);
        user.myMovement.DragActor(Vector2.zero);
        StartCoroutine(SpawnProjectiles(user));
    }

    private IEnumerator SpawnProjectiles(Actor user)
    {

        float[,] offset = new float[,] { { -offsetValue, 0 }, { offsetValue, 0 }, 
                                         { 0, offsetValue }, { 0, -offsetValue } };

        for (int i = 0; i < numProjectiles; i++)
        {
            Vector2 userPos = user.gameObject.transform.position;
            Vector2 projPos = new Vector2(userPos.x + offset[i, 0], userPos.y + offset[i, 1]);
            GameObject ghostKnightProjectile = Instantiate(this.toInstantiateProjectile, projPos, Quaternion.identity);
            this.projectileManager.Add(ghostKnightProjectile);
        }
        StartCoroutine(DestroyProjectiles());
        yield return new WaitForSeconds(this.projectileSpawnTime);
        isFinished = true;
    }

    private IEnumerator DestroyProjectiles()
    {
        yield return new WaitForSeconds(this.projectileDuration);
        for (int i = 0; i < this.projectileManager.Count; i++)
        {
            GameObject toDestroy = this.projectileManager[i];
            Destroy(toDestroy);
        }
        this.projectileManager.Clear();
    }
}
