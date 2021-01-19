using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Phase1 Projectile Attack.
public class GluttonyProjectileP1: ActorAbilityFunction<Actor, int>
{
    //Where gluttony will move towards before initiating the attack.
    [Tooltip("Where the user will move towards in the global space in order to start the ability.")]
    public Vector2 centerPos = Vector2.zero;
    //The projectile that gluttony will spawn. Must have an ActorMovement component.
    [Tooltip("The projectile gluttony will spawn. The prefab must have its own ActorMovement component.")]
    public GameObject toInstantiateProjectile;
    //How long this entire process should take.
    [SerializeField]
    [Tooltip("How long the ability should take to execute as a whole.")]
    float duration = 10f;
    //Time it will take to spawn the projectiles
    [SerializeField]
    [Tooltip("Of the duration time, how much goes towards spawning projectiles. Must be < duration.")]
    float projectileSpawnTime = 2f;
    //Delay before projectile spawning starts
    [SerializeField]
    [Tooltip("The delay between reaching the centerPos and spawningProjectiles. Must be < duration.")]
    float projectileDelay = 1f;
    //Number of projectiles to spawn.
    [Tooltip("The number of projectiles the user will spawn.")]
    public int numProjectiles = 8;
    //A list that is used to manage the projectiles spawned by this user
    List<GameObject> projectileManager;

    //Initialize monobehavior fields
    void Start()
    {
        projectileManager = new List<GameObject>();
    }

    //Similar to ActorAbilityFunction invoke but checks the isFinished flag.
    public override void Invoke(ref Actor user)
    {
        if(this.usable && this.isFinished)
        {
            StartCoroutine(coolDown(cooldownPeriod));
            InternInvoke(user);
        }
    }

    /*Internal invoke does some checks to make sure variables values are properly set
    It will also clear any projectiles from the scene this ability spawned in preparation for 
    spawning new projectiles.*/
    protected override int InternInvoke(params Actor[] args)
    {
        if (duration <= 0f || projectileSpawnTime <= 0f)
        {
            Debug.Log("GluttonPhaseChange: duration/projectileSpawn must be greater than 0");
            duration = 10f;
            projectileSpawnTime = 2f;
        }
        else if (duration <= projectileDelay + projectileSpawnTime)
        {
            Debug.Log("GluttonPhaseChange: duration must be greater than spawnTime + projectileDelay.");
            duration = 10f;
            projectileSpawnTime = 2f;
            projectileDelay = 1f;
        }

        for (int i = 0; i < projectileManager.Count; i++)
        {
            GameObject toDestroy = projectileManager[i];
            Destroy(toDestroy);
        }
        projectileManager.Clear();
        
        StartCoroutine(args[0].myMovement.LockActorMovement(duration));
        StartCoroutine(MoveToCenter(args[0]));
        return 0;
    }

    /*Part 1 of this two part ability
    This coroutine will move the user to the position dictates by the memebr variable centerPos.
    The speed of the movement is determined by the duration of this entire ability subtracted 
    from the time spent managing the projectiles.
    Therefore the speed the user moves is variable and will either be slower or faster 
    depening on distance from centerPos*/
    IEnumerator MoveToCenter(Actor user)
    {
        Vector2 direction = centerPos - new Vector2(user.gameObject.transform.position.x, user.gameObject.transform.position.y);
        direction.Normalize();
        float distance = Vector2.Distance(centerPos, user.gameObject.transform.position);
        float speed = distance / (duration - projectileSpawnTime - projectileDelay);
        user.myMovement.DragActor(direction * speed);
        yield return new WaitForSeconds(duration - projectileSpawnTime - projectileDelay);
        StartCoroutine(SpawnProjectiles(user));
    }

    /*Part 2 of this 2 part ability
    This Coroutine will spawn projectiles around the user after they ahve reached the center.
    Projectiles are spawned one after another with a minor delay inbetween spawns.*/
    IEnumerator SpawnProjectiles(Actor user)
    {
        yield return new WaitForSeconds(projectileDelay);
        int i = 0;
        float dtheta = (2/numProjectiles) * Mathf.PI; //(360/angle) * (pi/180)
        while(i < numProjectiles)
        {
            Vector2 direction = new Vector2(Mathf.Cos(i*dtheta), Mathf.Sin(i*dtheta)); 
            GameObject gluttonyProjectile = Instantiate(toInstantiateProjectile, user.gameObject.transform.position, Quaternion.identity);
            ActorMovement currProjectile = gluttonyProjectile.GetComponent<ActorMovement>();
            projectileManager.Add(gluttonyProjectile);
            currProjectile.DragActor(direction);
            yield return new WaitForSeconds(projectileSpawnTime/numProjectiles);
        }
        if (cooldownPeriod > duration)
        {
            yield return new WaitForSeconds(cooldownPeriod - duration);
        }
        this.isFinished = true;
    }
}
