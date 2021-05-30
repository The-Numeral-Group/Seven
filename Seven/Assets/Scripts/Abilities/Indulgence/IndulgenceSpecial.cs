using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndulgenceSpecial : ActorAbilityFunction<Actor, int>
{
    [Range(0.1f, 10f)]
    public float rangeToStop = 5f;
    public float suckSpeed = 7f;
    [Range(1f, 10f)]
    public float suckDuration = 5f;
    [Range(0.1f, 2f)]
    public float timeToReachCenter = 1f;
    [Range(0.1f, 2f)]
    public float projectileSpawnTime = 1f;
    [Range(0f, 5f)]
    public float projSpeed = 5;
    public GameObject indulgenceProjectilePrefab;
    public GameObject SuckEFX;
    public Vector2 centerOfArena = Vector2.zero;
    protected static List<GameObject> PROJECTILE_MANAGER;
    float totalDuration;
    Actor target;
    IEnumerator DragRoutine;
    IEnumerator MovementLock;
    IEnumerator ProjectileRoutine;
    IEnumerator FinishRoutine;

    void Awake()
    {
        target = null;
        totalDuration = suckDuration + timeToReachCenter + projectileSpawnTime;
        DragRoutine = DragTarget();
        ProjectileRoutine = SpawnProjectiles();
        FinishRoutine = FinishSequence();
        if (IndulgenceSpecial.PROJECTILE_MANAGER == null)
        {
            IndulgenceSpecial.PROJECTILE_MANAGER = new List<GameObject>();
        }
        SuckEFX.SetActive(false);
    }
    public override void Invoke(ref Actor user)
    {
        Debug.LogWarning("IndulgenceCharge: please use the Invoke(ref actor, param object[] args)");
    }

    public override void Invoke(ref Actor user, params object[] args)
    {
        this.user = user;
        //by default, Invoke just does InternInvoke with the provided arguments
        //it's also just implicitly convert the args and give it to InternInvoke
        if(usable && isFinished)
        {
            isFinished = false;
            InternInvoke(easyArgConvert(args));
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    protected override int InternInvoke(params Actor[] args)
    {
        target = args[0];
        MovementLock = this.user.myMovement.LockActorMovement(totalDuration);
        DragRoutine = DragTarget();
        ProjectileRoutine = SpawnProjectiles();
        FinishRoutine = FinishSequence();
        StartCoroutine(MovementLock);
        StartCoroutine(MoveToCenter());
        return 0;
    }

    protected virtual void CleanProjectiles()
    {
        for (int i = 0; i < IndulgenceSpecial.PROJECTILE_MANAGER.Count; i++)
        {
            GameObject toDestroy = IndulgenceSpecial.PROJECTILE_MANAGER[i];
            Destroy(toDestroy);
        }
        IndulgenceSpecial.PROJECTILE_MANAGER.Clear();
    }

    void FinishAbilitySequence()
    {
        StopCoroutine(MovementLock);
        StopCoroutine(DragRoutine);
        StopCoroutine(ProjectileRoutine);
        StopCoroutine(FinishRoutine);
        this.user.myAnimationHandler.Animator.SetBool("suck", false);
        SuckEFX.SetActive(false);
        StartCoroutine(this.user.myMovement.LockActorMovement(-1f));
        CleanProjectiles();
        isFinished = true;
    }
    IEnumerator MoveToCenter()
    {
        float distance = Vector2.Distance(this.user.transform.position, centerOfArena);
        Vector2 direction = (centerOfArena - new Vector2(this.user.transform.position.x, this.user.transform.position.y)).normalized;
        this.user.myAnimationHandler.Flip(direction);
        this.user.myAnimationHandler.Animator.SetBool("walking", true);
        float speed = distance / timeToReachCenter;
        this.user.myMovement.DragActor(direction * speed);
        yield return new WaitForSeconds(timeToReachCenter);
        this.user.myMovement.DragActor(Vector2.zero);
        this.user.myAnimationHandler.Animator.SetBool("walking", false);
        StartCoroutine(ProjectileRoutine);
    }

    IEnumerator SpawnProjectiles()
    {
        float dtheta = 2f * Mathf.PI / 8f;
        int skipIndex = Random.Range(0, 7);
        this.user.myAnimationHandler.Animator.SetTrigger("projectile_attack");
        for (int i = 0; i < 8; i++)
        {
            if (i ==  skipIndex)
            {
                continue;
            }
            Vector3 direction = new Vector3(Mathf.Cos(i * dtheta), Mathf.Sin(i * dtheta), 0);
            GameObject indulgenceProjectile = Instantiate(indulgenceProjectilePrefab, this.user.transform.position + direction,Quaternion.identity);
            IndulgenceSpecial.PROJECTILE_MANAGER.Add(indulgenceProjectile);
            indulgenceProjectile.GetComponent<ActorMovement>().DragActor(direction * projSpeed);
        }
        yield return new WaitForSeconds(projectileSpawnTime);
        StartCoroutine(DragRoutine);
        StartCoroutine(FinishRoutine);
    }
    IEnumerator DragTarget()
    {
        SuckEFX.SetActive(true);
        this.user.myAnimationHandler.Animator.SetBool("suck", true);
        while(target != null)
        {
            Vector2 destination = (this.user.transform.position - this.target.gameObject.transform.position).normalized;
            destination = destination * suckSpeed;
            this.target.myMovement.DragActor(destination);
            float distance = Vector2.Distance(this.user.transform.position, this.target.gameObject.transform.position);
            if (distance <= rangeToStop)
            {
                Debug.Log("SStopping");
                StopCoroutine(FinishRoutine);
                FinishAbilitySequence();
                break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator FinishSequence()
    {
        yield return new WaitForSeconds(suckDuration);
        StopCoroutine(DragRoutine);
        FinishAbilitySequence();
    }

    /*void OnCollisionEnter2D(Collision2D collider)
    {
        if (!this.isFinished && collider.gameObject == this.target.gameObject)
        {
             var enemyHealth = collider.gameObject.GetComponent<ActorHealth>();

            //or a weakpoint if there's no regular health
            if(enemyHealth == null){collider.gameObject.GetComponent<ActorWeakPoint>();}

            //if the enemy can take damage (if it has an ActorHealth component),
            //hurt them. Do nothing if they can't take damage.
            if(enemyHealth != null)
            {
                enemyHealth.takeDamage(1);
            }
        }
    }*/
}
