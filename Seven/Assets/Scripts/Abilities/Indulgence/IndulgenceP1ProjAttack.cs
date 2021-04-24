using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndulgenceP1ProjAttack : ActorAbilityFunction<Vector2, int>
{
    public bool clearProjectilesOnUse = true;
    public float projSpeed = 6f;
    [Range(0f, 5f)]
    public int numProjectiles = 5;
    [Range(0f, 2 * Mathf.PI)]
    public float projectileSpread = Mathf.PI/3;
    public GameObject projPrefab;
    public bool lockActor = true;
    [Range(0f, 5f)]
    public float projectileSpawnTime = 2f;
    protected static List<GameObject> PROJECTILE_MANAGER;
    IEnumerator projRoutine;
    IEnumerator movementLock;
    void Awake()
    {
        if (IndulgenceP1ProjAttack.PROJECTILE_MANAGER == null)
        {
            IndulgenceP1ProjAttack.PROJECTILE_MANAGER = new List<GameObject>();
        }
        projRoutine = CreateProjectiles(Vector2.zero);
    }

    public override void Invoke(ref Actor user)
    {
        Debug.LogWarning("IndulgenceP1ProjAttack: please use the Invoke(ref actor, param object[] args)");
    }

    public override void Invoke(ref Actor user, params object[] args)
    {
        this.user = user;
        if(usable && this.isFinished)
        {
            isFinished = false;
            InternInvoke(easyArgConvert(args));
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    protected override int InternInvoke(params Vector2[] args)
    {
        if (clearProjectilesOnUse)
        {
            CleanProjectiles();
        }
        Vector2 initialTravelDirection = args[0].normalized;
        StopCoroutine(projRoutine);
        projRoutine = CreateProjectiles(initialTravelDirection);
        if (lockActor)
        {
            if (movementLock != null)
            {
                StopCoroutine(movementLock);
            }
            movementLock = this.user.myMovement.LockActorMovement(projectileSpawnTime);
            StartCoroutine(movementLock);
            this.user.myMovement.DragActor(Vector2.zero);
        }
        StartCoroutine(projRoutine);
        return 0;
    }

    protected virtual void CleanProjectiles()
    {
        for (int i = 0; i < IndulgenceP1ProjAttack.PROJECTILE_MANAGER.Count; i++)
        {
            GameObject toDestroy = IndulgenceP1ProjAttack.PROJECTILE_MANAGER[i];
            Destroy(toDestroy);
        }
        IndulgenceP1ProjAttack.PROJECTILE_MANAGER.Clear();
    }

    IEnumerator CreateProjectiles(Vector2 initialTravelDirection)
    {
        Vector2[] projDirections = new Vector2[numProjectiles];
        GameObject[] projectilesArray = new GameObject[numProjectiles];
        float delta = projectileSpread / (float)numProjectiles;
        float delayBetweenSpawns = projectileSpawnTime / (float)numProjectiles;
        int evenCount = 1;
        int oddCount = 1;
        GameObject indulgenceProjectile = Instantiate(projPrefab, this.user.transform.position, Quaternion.identity);
        IndulgenceP1ProjAttack.PROJECTILE_MANAGER.Add(indulgenceProjectile);
        projectilesArray[0] = indulgenceProjectile;
        projDirections[0] = initialTravelDirection;
        indulgenceProjectile.transform.position = 
            this.user.transform.position + (2 * new Vector3(initialTravelDirection.x, initialTravelDirection.y, 0f));
        yield return new WaitForSeconds(delayBetweenSpawns);
        for (int i = 1; i < numProjectiles; i++)
        {
            Vector2 newDirection;
            float dtheta;
            if (i % 2 != 0)
            {
                dtheta = delta * oddCount * -1;
                oddCount++;
            }
            else
            {
                dtheta = delta * evenCount;
                evenCount++;
            }
            newDirection = new Vector2(initialTravelDirection.x * Mathf.Cos(dtheta) - initialTravelDirection.y * Mathf.Sin(dtheta),
                            initialTravelDirection.x * Mathf.Sin(dtheta) + initialTravelDirection.y * Mathf.Cos(dtheta));
            projDirections[i] = newDirection;
            indulgenceProjectile = Instantiate(projPrefab, this.user.transform.position, Quaternion.identity);
            IndulgenceP1ProjAttack.PROJECTILE_MANAGER.Add(indulgenceProjectile);
            projectilesArray[i] = indulgenceProjectile;
            indulgenceProjectile.transform.position = 
                this.user.transform.position + (2 * new Vector3(newDirection.x, newDirection.y, 0f));
            yield return new WaitForSeconds(delayBetweenSpawns);
        }

        for (int i = 0; i < numProjectiles; i++)
        {
            if (projectilesArray[i] != null)
            {
                projectilesArray[i].GetComponent<ActorMovement>().DragActor(projDirections[i] * projSpeed);
            }
        }
        isFinished = true;
    }

}
