using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathFireWall : ActorAbilityFunction<GameObject, int>
{
    //FIELDS---------------------------------------------------------------------------------------
    [Header("Firewall Settings.")]
    [Tooltip("The closest the user should appear to its target after a teleport.")]
    public float teleMinDist = 10f;

    [Tooltip("The farthest the user should appear from its target after a teleport.")]
    public float teleMaxDist = 20f;

    [Tooltip("The delay between teleport-firewall combos.")]
    public float attackDelay = 0.5f;

    [Tooltip("The delay between a teleport and its follow-up fire launch.")]
    public float launchDelay = 0.5f;

    [Tooltip("The number of firewalls launched by this ability (minimum 1).")]
    public int attackCount = 4;

    [Header("Generic Projectile Settings.")]
    [Tooltip("The projectile that this ability launches (the effects of the projectile" +
        " can be configured by that game object).")]
    public GameObject projectile;

    [Tooltip("Where the projectile should spawn relative to the user's faceAnchor.")]
    public Vector2 projectileScale = new Vector2(1, 1);

    [Tooltip("Which direction the ability should be launched in if the user doesn't" +
        " specify a Transform, Vector2 direction, Actor, or gameObject target")]
    public Vector2 projectileDirection = Vector2.left;

    //the last object that had firewalls launched at
    public GameObject lastTarget { get; private set; }

    /*[Tooltip("Whether or not the animation for this attack is at a good spot to" + 
        " actually create the projectile. Please do not manually edit this.")]*/
    //helper bool to track animation progress, if any
    private bool fireAnimClear = false;
    //The single firewall launcher
    WrathFireSingle single;

    private IEnumerator MovementLockroutine;
    private Vector3 originalScale;

    //METHODS--------------------------------------------------------------------------------------
    // Called the first frame of the scene
    //Used to initialize the single-fire ability
    void Start()
    {
        lastTarget = null;
        single = this.gameObject.AddComponent<WrathFireSingle>();
        single.Init(this);
    }

    /*Activates the ability with no arguments. In this case, it will default the target to
    the player*/
    public override void Invoke(ref Actor user)
    {
        this.user = user;
        if (usable)
        {
            isFinished = false;

            InternInvoke(GameObject.FindWithTag("Player"));
        }

    }

    /*Same as the above method, but with a provided vector position*/
    public override void Invoke(ref Actor user, params object[] args)
    {
        this.user = user;
        //by default, Invoke just does InternInvoke with the provided arguments
        //it's also just implicitly convert the args and give it to InternInvoke
        if (usable)
        {
            isFinished = false;

            originalScale = this.user.gameObject.transform.localScale;
            this.user.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            //make sure the argument is a gameObject of some sort
            if (args[0] is GameObject)
            {
                InternInvoke((args[0] as GameObject));
            }
            else if (args[0] is MonoBehaviour)
            {
                InternInvoke((args[0] as MonoBehaviour).gameObject);
            }
            else
            {
                Debug.LogError("WrathFireWall: firewall launched with improper target," +
                    " must be GameObject or Monobehaviour");
            }
        }
    }
    /*protected override int InternInvoke(params Actor[] args)
    {
        // Making sure the movementDirection and dragDirection have been resetted.
        args[0].myMovement.MoveActor(Vector2.zero);
        args[0].myMovement.DragActor(Vector2.zero);

        StartCoroutine(args[0].myMovement.LockActorMovement(this.duration));
        StartCoroutine(FireWallFinished(args[0]));
        return 0;
    }*/
    
    //wrapper for a coroutine that handles the duration of the actual laser
    protected override int InternInvoke(params GameObject[] args)
    {
        MovementLockroutine = this.user.myMovement.LockActorMovement(Mathf.Infinity); // Lock Wrath's movement inf, unlock when done.
        lastTarget = args[0];
        StartCoroutine(FireInvokation(args[0]));
        StartCoroutine(MovementLockroutine);
        return 1;
    }

    //handles the actual launching of the firewalls
    IEnumerator FireInvokation(GameObject target)
    {
        // MISSING ANIM ------------------------------------------------------
        fireAnimClear = !user.myAnimationHandler.TrySetTrigger("Wrath_Shoot");

        yield return new WaitUntil(() => fireAnimClear == true);
        fireAnimClear = false;

        //Step 1: Launch the first fire wall
        single.Invoke(ref this.user, target.transform.position, LAUNCH_MODE.POINT);

        //Step 2: Do the rest of the ability lol
        for (int i = 0; i < attackCount - 1; ++i)
        {
            //Step 2.1: Wait a little bit
            yield return new WaitForSeconds(attackDelay);

            //Step 2.2: Teleport to a random location near the target.
            var dest = target.transform.position + new Vector3(
                Random.Range(teleMinDist, teleMaxDist) * (Random.Range(0, 2) * 2 - 1),
                Random.Range(teleMinDist, teleMaxDist) * (Random.Range(0, 2) * 2 - 1),
                0f
            );
            yield return Ego2Movement.EgoTeleport(dest, user.gameObject);

            //Step 2.25: Animate the attack
            // MISSING ANIM ------------------------------------------------------
            fireAnimClear = !user.myAnimationHandler.TrySetTrigger("Wrath_Shoot");

            yield return new WaitUntil(() => fireAnimClear == true);
            fireAnimClear = false;

            //Step 2.3: Launch the projectile
            single.Invoke(ref this.user, target.transform.position, LAUNCH_MODE.POINT);

            //Repeat
        }

        //Step 3: Wait a little bit again
        yield return new WaitForSeconds(attackDelay);

        // Teleport Wrath to Player
        this.user.transform.position = target.transform.position;
        StopCoroutine(MovementLockroutine); // Unlock wrath's movement.
        this.user.gameObject.transform.localScale = originalScale;
        isFinished = true;
        Debug.Log("WrathFireWall: ability done");
    }

    public void SignalFireAnim()
    {
        fireAnimClear = true;
    }
}

internal class WrathFireSingle : ProjectileAbility
{
    //FIELDS---------------------------------------------------------------------------------------
    //the WrathFireWall to base this ability's attacks off of
    public WrathFireWall wrap { get; set; }

    //METHODS--------------------------------------------------------------------------------------
    //Called on the first frame of the scene
    //Used to initialize this object on startup
    public void Init(WrathFireWall wrap)
    {
        //save the wrapper using this ability
        this.wrap = wrap;

        //initialize public variables with the wrapper's values
        projectile = wrap.projectile;
        projectileScale = wrap.projectileScale;
        projectileDirection = wrap.projectileDirection;

        // This currently is not affecting the volume for some reason.
        AudioSource fireSFX = projectile.gameObject.GetComponent<AudioSource>();
        if (fireSFX)
        {
            fireSFX.volume = GameSettings.MASTER_VOLUME * GameSettings.SFX_VOLUME / 4; // The sound source is already too loud, so dividing by 4
        }
    }

    /*Launch the projectile. The anticipated argument is the gameObject being shot at. The 
    gameObject may-or-may-not have an ActorHealth component.*/
    protected override int InternInvoke(params Vector2[] args)
    {
        projObj.GetComponent<FilterProjectile>().Launch(args[0], launchMode, wrap.lastTarget);

        isFinished = true;

        return 0;
    }
}
