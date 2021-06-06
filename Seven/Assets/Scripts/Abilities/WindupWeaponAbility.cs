using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindupWeaponAbility : WeaponAbility
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("How long the windup for the attack is")]
    public float windupDelay = 1.5f;

    //[Tooltip("The trigger to start an animation to play during this attack, if any.")]
    //public string animTrigger = "";
    [Tooltip("The name of a sound clip to play when the attack becomes active.")]
    public string attackSound;

    [Tooltip("The name of the sound clip to play as the weapon is winding up")]
    public string windupSound;

    [Header("Screen Shake Settings")]
    [Tooltip("Whether this attack should cause the screen to shake when it is used.")]
    public bool shouldShake = false;

    [Tooltip("The magnitude of the screen shake during the windup.")]
    public float windShake = 0.1f;

    [Tooltip("The magnitude of the screen shake during the actual attack.")]
    public float attackShake = 1f;

    //a reference to the camera to cause the screenshake
    protected BaseCamera cameraFunc;

    //METHODS--------------------------------------------------------------------------------------
    // Start is called on the first frame of the scene
    protected override void Start()
    {
        //get the camera
        cameraFunc = GameObject.FindWithTag("MainCamera").GetComponent<BaseCamera>();

        //do the rest ofthe initialization
        base.Start();
    }

    /*Wrapper for a delayedAttack()*/
    protected override int InternInvoke(params Actor[] args)
    {
        if(this.animTrigger.Length != 0)
        {
            user.myAnimationHandler.TrySetTrigger(this.animTrigger);
        }

        StartCoroutine(delayedAttack(args));
        return 0;
    }

    /*More closely follows the standard InternIvoke of WeaponAbility, but waits for a short delay
    before the actual attack is performed*/
    protected virtual IEnumerator delayedAttack(params Actor[] args)
    {
        //clean the old attack instance
        this.hitConnected = false;
        StopCoroutine(sheathe);

        //play the windup sound
        if(windupSound.Length != 0){ user.mySoundManager?.PlaySound(windupSound); }

        //wait for the windup
        //if the screen should shake, start the minor shake
        if(shouldShake){ cameraFunc.Shake(windupDelay, windShake); }
        //yield return new WaitForSeconds(windupDelay);

        yield return this.trackedWindup(this.windupDelay, args[0]);

        //if(windupSound.Length != 0){ user.mySoundManager?.StopSound(windupSound); }

        //then do the rest of the attack as normal
        sheathe = SheatheWeapon();
        weaponObject.SetActive(true);
        SpawnWeapon(args);

        //start the screen shake here, since we can't override SheathWeapon()
        if(shouldShake){ cameraFunc.Shake(duration, attackShake); }
        //also play the sound here
        if(attackSound.Length != 0){ user.mySoundManager?.PlaySound(attackSound); }
        StartCoroutine(sheathe);
    }

    protected IEnumerator trackedWindup(float duration, Actor target)
    {
        for(float clock = 0f; clock < duration; clock += Time.deltaTime)
        {
            user.SendMessage("RotateActor", 
                (Vector2)(target.gameObject.transform.position 
                    - user.faceAnchor.position).normalized);
            user.SendMessage("animateWalk");

            yield return null;
        }
    }
}
