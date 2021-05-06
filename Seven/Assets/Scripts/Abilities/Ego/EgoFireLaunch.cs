using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EgoFireLaunch : ActorAbilityFunction<GameObject, int>
{
    //FIELDS---------------------------------------------------------------------------------------
    [Header("Firewall Settings.")]
    [Tooltip("The closest the user should appear to its target after a teleport.")]
    public float teleMinDist = 10f;

    [Tooltip("The farthest the user should appear from its target after a teleport.")]
    public float teleMaxDist = 20f;

    [Tooltip("The delay between teleport-firewall combos.")]
    public float attackDelay = 0.5f;

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

    [Tooltip("Whether or not the animation for this attack is at a good spot to" + 
        " actually create the projectile. Please do not manually edit this.")]
    public bool animClear = false;

    //The single firewall launcher
    EgoFireSingle single;

    //METHODS--------------------------------------------------------------------------------------
    // Called the first frame of the scene
    //Used to initialize the single-fire ability
    void Start()
    {
        single = this.gameObject.AddComponent<EgoFireSingle>();
        single.Init(this);
    }

    /*Activates the ability with no arguments. In this case, it will default the target to
    the player*/
    public override void Invoke(ref Actor user)
    {
        this.user = user;
        if(usable)
        {
            isFinished = false;
            InternInvoke(GameObject.FindWithTag("Player"));
            StartCoroutine(coolDown(cooldownPeriod));
        }
        
    }

    /*Same as the above method, but with a provided vector position*/
    public override void Invoke(ref Actor user, params object[] args)
    {
        this.user = user;
        //by default, Invoke just does InternInvoke with the provided arguments
        //it's also just implicitly convert the args and give it to InternInvoke
        if(usable)
        {
            isFinished = false;

            //make sure the argument is a gameObject of some sort
            if(args[0] is GameObject)
            {
                InternInvoke((args[0] as GameObject));
            }
            else if(args[0] is MonoBehaviour)
            {
                InternInvoke((args[0] as MonoBehaviour).gameObject);
            }
            else
            {
                Debug.LogError("EgoFireLaunch: firewall launched with improper target," + 
                    " must be GameObject or Monobehaviour");
            }
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    //wrapper for a coroutine that handles the duration of the actual laser
    protected override int InternInvoke(params GameObject[] args)
    {
        usable = false;
        StartCoroutine(FireInvokation(args[0]));
        return 1;
    }

    //handles the actual launching of the firewalls
    IEnumerator FireInvokation(GameObject target)
    {
        //Step 1: Launch the first fire wall
        single.Invoke(ref this.user, target.transform.position, LAUNCH_MODE.POINT);

        //Step 2: Do the rest of the ability lol
        for(int i = 0; i < attackCount - 1; ++i)
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
            animClear = false;
            user.myAnimationHandler.TrySetTrigger("ego_shoot");
            Debug.Log($"EgoFireLaunch: animClear is {animClear}");

            yield return new WaitUntil( () => animClear );
            Debug.Log($"EgoFireLaunch: animClear is {animClear}");

            //Step 2.3: Launch the projectile
            single.Invoke(ref this.user, target.transform.position, LAUNCH_MODE.POINT);
            
            //Repeat
        }

        //Step 3: Wait a little bit again
        yield return new WaitForSeconds(attackDelay);

        //Step 4: Turn on the cooldown. Attack is done.
        StartCoroutine(coolDown(cooldownPeriod));

        isFinished = true;
        ///DEBUG
        Debug.Log("EgoFireLaunch: ability done");
        ///DEBUG
    }
}

internal class EgoFireSingle : ProjectileAbility
{
    //FIELDS---------------------------------------------------------------------------------------
    //the EgoFireLaunch to base this ability's attacks off of
    public EgoFireLaunch wrap { get; set; }

    //METHODS--------------------------------------------------------------------------------------
    //Called on the first frame of the scene
    //Used to initialize this object on startup
    public void Init(EgoFireLaunch wrap)
    {
        //save the wrapper using this ability
        this.wrap = wrap;

        //initialize public variables with the wrapper's values
        projectile = wrap.projectile;
        projectileScale = wrap.projectileScale;
        projectileDirection = wrap.projectileDirection;
    }

    protected override int InternInvoke(params Vector2[] args)
    {
        return base.InternInvoke(args);
    }
}
