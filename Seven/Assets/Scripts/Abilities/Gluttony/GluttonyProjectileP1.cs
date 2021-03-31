using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Document Link: https://docs.google.com/document/d/1MAwGN52BuCVGs3T8_-DZKzz-iCr16ISm1hs8UtdvsXg/edit?usp=sharing
//Phase1 Projectile Attack.
public class GluttonyProjectileP1: ActorAbilityFunction<Actor, int>
{
    //Where gluttony will move towards before initiating the attack.
    [Tooltip("Where the user will move towards in the global space in order to start the ability.")]
    public Vector2 centerPos = Vector2.zero;
    //The projectile that gluttony will spawn. Must have an ActorMovement component.
    [Tooltip("The projectile gluttony will spawn. The prefab must have its own ActorMovement component.")]
    public GameObject toInstantiateProjectile;
    //Time it will take to spawn the projectiles
    [SerializeField]
    [Tooltip("Of the duration time, how much goes towards spawning projectiles. Must be < duration.")]
    protected float projectileSpawnTime = 2f;
    //Delay before projectile spawning starts
    [SerializeField]
    [Tooltip("The delay between reaching the centerPos and spawningProjectiles. Must be < duration.")]
    protected float projectileDelay = 1f;
    //Number of projectiles to spawn.
    [Tooltip("The number of projectiles the user will spawn.")]
    public int numProjectiles = 8;
    //A list that is used to manage the projectiles spawned by this user
    protected static List<GameObject> PROJECTILE_MANAGER = new List<GameObject>();
    //reference to the animationd handler. must be cast as gluttony animation handler.
    protected GluttonyP1AnimationHandler gluttonyAnimationHandler;

    protected void Awake()
    {
        if (projectileDelay < 0f || projectileSpawnTime <= 0f)
        {
            Debug.LogWarning("GluttonPhaseChange: duration/projectileSpawn must be greater than 0");
            projectileDelay = 1f;
            projectileSpawnTime = 2f;
        }
    }

    //Similar to ActorAbilityFunction invoke but checks the isFinished flag.
    public override void Invoke(ref Actor user)
    {
        if(this.usable && this.isFinished)
        {
            this.isFinished = false;
            gluttonyAnimationHandler = user.myAnimationHandler as GluttonyP1AnimationHandler;
            StartCoroutine(coolDown(cooldownPeriod));
            InternInvoke(user);
        }
    }

    /*Internal invoke does some checks to make sure variables values are properly set
    It will also clear any projectiles from the scene this ability spawned in preparation for 
    spawning new projectiles.*/
    protected override int InternInvoke(params Actor[] args)
    {
        for (int i = 0; i < GluttonyProjectileP1.PROJECTILE_MANAGER.Count; i++)
        {
            GameObject toDestroy = GluttonyProjectileP1.PROJECTILE_MANAGER[i];
            Destroy(toDestroy);
        }
        GluttonyProjectileP1.PROJECTILE_MANAGER.Clear();
        
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
        Vector2 direction = centerPos - new Vector2(user.gameObject.transform.position.x, 
                                                     user.gameObject.transform.position.y);
        direction.Normalize();
        float distance = Vector2.Distance(centerPos, user.gameObject.transform.position);
        float time = distance / user.myMovement.speed;
        StartCoroutine(user.myMovement.LockActorMovement(time + projectileDelay + projectileSpawnTime));
        gluttonyAnimationHandler.AnimateWalk(true, direction);
        user.myMovement.DragActor(direction * user.myMovement.speed);
        yield return new WaitForSeconds(time);
        gluttonyAnimationHandler.AnimateWalk(false, direction);
        user.myMovement.DragActor(Vector2.zero);
        StartCoroutine(SpawnProjectiles(user));
    }

    /*Part 2 of this 2 part ability
    This Coroutine will spawn projectiles around the user after they ahve reached the center.
    Projectiles are spawned one after another with a minor delay inbetween spawns.*/
    protected virtual IEnumerator SpawnProjectiles(Actor user)
    {
        yield return new WaitForSeconds(projectileDelay);
        gluttonyAnimationHandler.Animator.SetTrigger("Projectile");
        int i = 0;
        float dtheta = (2f/numProjectiles) * Mathf.PI; //(360/angle) * (pi/180)
        while(i < numProjectiles)
        {
            Vector2 direction = new Vector2(Mathf.Cos(i*dtheta), Mathf.Sin(i*dtheta));
            GameObject gluttonyProjectile = Instantiate(toInstantiateProjectile, 
                                            user.gameObject.transform.position, Quaternion.identity);
            Physics2D.IgnoreCollision(user.gameObject.GetComponent<Collider2D>(), 
                gluttonyProjectile.GetComponent<Collider2D>());
            ActorMovement currProjectile = gluttonyProjectile.GetComponent<ActorMovement>();
            GluttonyProjectileP1.PROJECTILE_MANAGER.Add(gluttonyProjectile);
            currProjectile.DragActor(direction);
            yield return new WaitForSeconds(projectileSpawnTime/numProjectiles);
            i++;
        }

        this.isFinished = true;
    }
}
