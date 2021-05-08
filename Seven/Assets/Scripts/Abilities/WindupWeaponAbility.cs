using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindupWeaponAbility : WeaponAbility
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("How long the windup for the attack is")]
    public float windupDelay = 1.5f;

    [Tooltip("The trigger to start an animation to play during this attack, if any.")]
    public string animTrigger = "";

    [Header("Screen Shake Settings")]
    [Tooltip("Whether this attack should cause the screen to shake when it is used.")]
    public bool shouldShake = false;

    [Tooltip("The magnitude of the screen shake during the windup.")]
    public float windShake = 0.1f;

    [Tooltip("The magnitude of the screen shake during the actual attack.")]
    public float attackShake = 1f;

    //a reference to the camera to cause the screenshake
    private BaseCamera cameraFunc;

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
        if(animTrigger.Length != 0)
        {
            user.myAnimationHandler.TrySetTrigger(animTrigger);
        }

        StartCoroutine(delayedAttack(args));
        return 0;
    }

    /*More closely follows the standard InternIvoke of WeaponAbility, but waits for a short delay
    before the actual attack is performed*/
    IEnumerator delayedAttack(params Actor[] args)
    {
        //clean the old attack instance
        this.hitConnected = false;
        StopCoroutine(sheathe);

        //wait for the windup
        //if the screen should shake, start the minor shake
        if(shouldShake){ cameraFunc.Shake(windupDelay, windShake); }
        yield return new WaitForSeconds(windupDelay);

        //then do the rest of the attack as normal
        sheathe = SheatheWeapon();
        weaponObject.SetActive(true);
        SpawnWeapon(args);

        //start the screen shake here, since we can't override SheathWeapon()
        if(shouldShake){ cameraFunc.Shake(duration, attackShake); }
        StartCoroutine(sheathe);
    }
}
