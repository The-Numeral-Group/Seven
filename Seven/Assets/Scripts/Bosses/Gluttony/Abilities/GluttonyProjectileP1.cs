using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Phase1 Projectile Attack.
public class GluttonyProjectileP1: ActorAbilityFunction<Actor, int>
{
    //Where gluttony will move towards before initiating the attack.
    public Vector2 centerPos = Vector2.zero;
    //The projectile that gluttony will spawn. Must have an ActorMovement component.
    public GameObject toInstantiateProjectile;
    //How long this entire process should take.
    public float duration = 10f;
    //Time it will take to spawn the projectiles
    public float projectileSpawnTime = 2f;
    //Delay before projectile spawning starts
    public float projectileDelay = 1f;
    //Number of projectiles to spawn.
    public int numProjectiles = 8;
    private List<GameObject> projectileManager = new List<GameObject>();
    //Similar to base class invoke but usable is reset elsewhere
    public override IEnumerator coolDown(float cooldownDuration)
    {
        usable = false;
        yield return null;
        //yield return new WaitForSeconds(cooldownDuration);
        //usable = true;
    }
    public override void Invoke(ref Actor user)
    {
        if(usable)
        {
            InternInvoke(user);
            //StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    //Internal invoke does some checks to make sure variables values are properly set
    //It will also clear any projectiles from the scene this ability spawned in preparation for spawning new projectiles.
    protected override int InternInvoke(Actor[] args)
    {
        if (this.duration <= 0f || this.projectileSpawnTime <= 0f)
        {
            Debug.Log("GluttonPhaseChange: duration/projectileDelay must be greater than 0");
            this.duration = 10f;
            this.projectileSpawnTime = 2f;
        }

        for (int i = 0; i < this.projectileManager.Count; i++)
        {
            GameObject toDestroy = this.projectileManager[i];
            Destroy(toDestroy);
        }
        this.projectileManager.Clear();
        
        StartCoroutine(args[0].myMovement.LockActorMovement(this.duration));
        StartCoroutine(MoveToCenter(args[0]));
        return 0;
    }

    //This coroutine will move the user to the position dictates by the memebr variable centerPos.
    //The speed of the movement is determined by the duration of this entire ability subtracted from the time spend managing the projectiles.
    //Therefore the speed the actor moves is variable and will either be slower or faster depening on distance from centerPos;
    private IEnumerator MoveToCenter(Actor user)
    {
        Vector2 direction = this.centerPos - new Vector2(user.gameObject.transform.position.x, user.gameObject.transform.position.y);
        direction.Normalize();
        float distance = Vector2.Distance(this.centerPos, user.gameObject.transform.position);
        float speed = distance / (this.duration - this.projectileSpawnTime - this.projectileDelay);
        user.myMovement.DragActor(direction * speed);
        yield return new WaitForSeconds(this.duration - this.projectileSpawnTime - this.projectileDelay);
        StartCoroutine(SpawnProjectiles(user));
    }

    //This Coroutine will spawn projectiles around the user after they ahve reached the center.
    //Projectiles are spawned one after another with a minor delay inbetween spawns.
    private IEnumerator SpawnProjectiles(Actor user)
    {
        yield return new WaitForSeconds(this.projectileDelay);
        int i = 0;
        float dtheta = (2/this.numProjectiles) * Mathf.PI; //(360/angle) * (pi/180)
        while(i < this.numProjectiles)
        {
            Vector2 direction = new Vector2(Mathf.Cos(i*dtheta), Mathf.Sin(i*dtheta)); 
            GameObject gluttonyProjectile = Instantiate(this.toInstantiateProjectile, user.gameObject.transform.position, Quaternion.identity);
            ActorMovement currProjectile = gluttonyProjectile.GetComponent<ActorMovement>();
            this.projectileManager.Add(gluttonyProjectile);
            currProjectile.DragActor(direction);
            yield return new WaitForSeconds(this.projectileSpawnTime/this.numProjectiles);
        }
        if (cooldownPeriod > this.duration)
        {
            yield return new WaitForSeconds(cooldownPeriod - this.duration);
        }
        usable = true;
    }
}
