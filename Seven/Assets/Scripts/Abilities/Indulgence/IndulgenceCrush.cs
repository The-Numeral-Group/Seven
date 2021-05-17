using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndulgenceCrush : ActorAbilityFunction<Actor, int>
{
    public bool overrideCooldown { get; set; }
    public bool useTrackingCrush;
    public Vector3 shadowOffset;
    public float jumpSpeed = 20f;
    [SerializeField]
    [Range(0.1f, 5f)]
    float jumpDuration = 1f;
    [SerializeField]
    [Range(0.1f, 5f)]
    float trackDuration = 2f;
    [SerializeField]
    [Range(0.1f, 5f)]
    float crushDuration = 0.5f;
    [SerializeField]
    [Range(0.1f, 5f)]
    float crushDelay = 1f;
    [SerializeField]
    [Range(0.1f, 5f)]
    float finishDelay = 0.5f;
    public float totalDuration { get; protected set; }
    public GameObject shadowSpritePrefab;
    GameObject shadowSprite;
    ActorMovement shadowSpriteMovement;
    IEnumerator JumpRoutine;
    IEnumerator TrackRoutine;
    IEnumerator CrushRoutine;
    IEnumerator MovementLock;
    bool hasLanded;
    Actor abilityTarget;

    void Awake()
    {
        this.totalDuration = jumpDuration + trackDuration + crushDuration + crushDelay + finishDelay;
    }

    void Start()
    {
        if (shadowSpritePrefab == null)
        {
            Debug.LogWarning("IndulgenceCrush: Shadowsprite prefab not attached, disabling ability.");
            this.enabled = false;
        }
        else
        {
            shadowSprite = Instantiate(shadowSpritePrefab, this.gameObject.transform);
            //StartCoroutine(shadowSprite.GetComponent<ActorMovement>().LockActorMovement(-1f));
            shadowSprite.SetActive(false);
            shadowSpriteMovement = shadowSprite.GetComponent<ActorMovement>();
        }
    }

    public void SetTotalAbilityDuration(float jumpD = 0.5f, float trackD = 2.5f, float crushD = 0.5f, float cDelay = 1.0f, float finishD = 0.5f)
    {
        jumpDuration = jumpD;
        trackDuration = trackD;
        crushDuration = crushD;
        crushDelay = cDelay;
        finishDelay = finishD;
        this.totalDuration = jumpDuration + trackDuration + crushDuration + crushDelay + finishDelay;
    }

    void SetupColliders(bool value)
    {
        var actorColliders = this.user.gameObject.GetComponents<Collider2D>();
        foreach(var actorCollider in actorColliders)
        {
            if (actorCollider.gameObject.activeSelf)
            {
                actorCollider.enabled = value;
            }
        }
        var childColliders = this.user.gameObject.GetComponentsInChildren<Collider2D>();
        foreach(var actorCollider in childColliders)
        {
            if (actorCollider.gameObject.activeSelf)
            {
                actorCollider.enabled = value;
            }
        }
    }
    public override void Invoke(ref Actor user)
    {
        Debug.LogWarning("IndulgenceCrush: please use the Invoke(ref actor, param object[] args)");
    }

    public override void Invoke(ref Actor user, params object[] args)
    {
        this.user = user;
        //by default, Invoke just does InternInvoke with the provided arguments
        //it's also just implicitly convert the args and give it to InternInvoke
        if((usable || this.overrideCooldown) && isFinished)
        {
            isFinished = false;
            InternInvoke(easyArgConvert(args));
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    protected override int InternInvoke(params Actor[] args)
    {
        hasLanded = false;
        abilityTarget = args[0];
        shadowSprite.transform.parent = user.transform;
        SetupColliders(false);
        MovementLock = this.user.myMovement.LockActorMovementOnly(this.totalDuration * 3);
        JumpRoutine = JumpUp(args[0]);
        CrushRoutine = CrushAtShadow();
        if (useTrackingCrush)
        {
            TrackRoutine = TrackTargetWithShadow(args[0]);
        }
        else
        {
            TrackRoutine = PredictingCrush(args[0]);
        }
        StartCoroutine(MovementLock);
        StartCoroutine(JumpRoutine);
        return 0;
    }

    IEnumerator JumpUp(Actor target)
    {
        Vector3 launchPosition = this.user.transform.position;
        shadowSprite.transform.parent = null;
        Vector2 direction = new Vector2(0, 1) * jumpSpeed;
        this.user.myMovement.DragActor(direction);
        yield return new WaitForSeconds(jumpDuration);
        shadowSprite.transform.position = launchPosition;
        shadowSprite.SetActive(true);
        this.user.myMovement.DragActor(Vector2.zero);
        this.user.transform.position = new Vector3(this.user.transform.position.x, 
            100, this.user.transform.position.z);
        if (target.gameObject != null)
        {
            StartCoroutine(TrackRoutine);
        }
        StartCoroutine(CrushRoutine);
    }
    
    IEnumerator PredictingCrush(Actor target)
    {
        StartCoroutine(shadowSpriteMovement.LockActorMovementOnly(this.totalDuration - this.jumpDuration));
        Vector2 destination = target.transform.position + (5 * target.faceAnchor.localPosition) + shadowOffset;
        Vector2 shadowPosition = new Vector2(shadowSprite.transform.position.x, shadowSprite.transform.position.y);
        Vector2 direction = destination - shadowPosition;
        direction = direction.normalized;
        float trackSpeed = Vector2.Distance(destination, shadowPosition) / trackDuration;
        shadowSpriteMovement.DragActor(direction * trackSpeed);
        yield return null;
    }

    IEnumerator TrackTargetWithShadow(Actor target)
    {
        while (true && target.gameObject != null)
        {
            yield return new WaitForFixedUpdate();
            shadowSprite.transform.position = target.transform.position + shadowOffset;
        }
    }

    IEnumerator CrushAtShadow()
    {
        yield return new WaitForSeconds(trackDuration);
        StopCoroutine(TrackRoutine);
        //shadowSprite.GetComponent<ActorMovement>().DragActor(Vector2.zero);
        Vector2 destination = shadowSprite.transform.position - shadowOffset;
        shadowSpriteMovement.DragActor(Vector2.zero);
        this.user.transform.position = new Vector3(shadowSprite.transform.position.x, 
            destination.y + 100, this.user.transform.position.z);
        //Debug.Log("First:" + destination + ":" + this.user.transform.position);
        /*Vector2 direction = Vector2.down;
        float distance = Vector2.Distance(this.user.transform.position, destination);
        float speed = distance / crushDuration;*/
        //Debug.Log(distance + ":" + speed + ":" +crushDuration);
        yield return new WaitForSeconds(crushDelay);
        float currentTime = 0f;
        Vector2 startPosition = this.user.transform.position;
        while (currentTime <= crushDuration)
        {
            this.user.transform.position = Vector3.Lerp(startPosition, destination, currentTime / crushDuration);
            currentTime += 0.5f * Time.deltaTime;
            yield return null;
        }
        //this.user.myMovement.DragActor(direction * speed);
        //yield return new WaitForSeconds(crushDuration);
        this.user.myMovement.DragActor(Vector2.zero);
        hasLanded = true;
        this.user.myAnimationHandler.Animator.SetTrigger("landing");
        //Debug.Log("Second:" + destination + ":" + this.user.transform.position);
        StartCoroutine(shadowSpriteMovement.LockActorMovementOnly(-1f));
        shadowSprite.transform.parent = this.user.transform;
        shadowSprite.transform.localPosition = Vector3.zero;
        shadowSprite.SetActive(false);
        SetupColliders(true);
        Camera.main.GetComponent<BaseCamera>().Shake(2.0f, 0.2f);
        yield return new WaitForSeconds(finishDelay);
        Debug.Log("Third:" + destination + ":" + this.user.transform.position);
        AftermathOfCrush();
    }

    void AftermathOfCrush()
    {
        StopCoroutine(JumpRoutine);
        StopCoroutine(TrackRoutine);
        StopCoroutine(CrushRoutine);
        StopCoroutine(MovementLock);
        StartCoroutine(this.user.myMovement.LockActorMovementOnly(-1f));
        this.isFinished = true;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (this.isFinished || collider.gameObject != abilityTarget.gameObject || !hasLanded)
        {
            return;
        }
        var enemyHealth = collider.gameObject.GetComponent<ActorHealth>();

        //or a weakpoint if there's no regular health
        if(enemyHealth == null){collider.gameObject.GetComponent<ActorWeakPoint>();}

        //if the enemy can take damage (if it has an ActorHealth component),
        //hurt them. Do nothing if they can't take damage.
        if(enemyHealth != null){
            if (!enemyHealth.vulnerable)
            {
                return;
            }
            enemyHealth.takeDamage(1);
        }
    }
}
