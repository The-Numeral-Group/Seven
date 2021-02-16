using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : ActorAbilityFunction<Actor, int>
{

    [Tooltip("The gameobject that will handle player's interact")]
    public GameObject playerInteractableHitbox;

    public float duration;

    public Vector2 objectPositionScale = new Vector2(0.1f, 0.1f);

    protected GameObject interactObject;

    protected IEnumerator sheathe;

    public bool hitConnected { get; set; }

    public Actor user { get; protected set; }

    protected virtual void Awake()
    {
        this.hitConnected = false;
        sheathe = SheatheObject();
    }

    protected virtual void Start()
    {
        interactObject = Instantiate(playerInteractableHitbox, this.gameObject.transform);

        interactObject.transform.localPosition = objectPositionScale;

        interactObject.transform.parent = this.gameObject.transform;
        interactObject.SetActive(false);
    }


    /*Similar to ActorAbilityFunction Invoke
    passes an actors movement component to InternalInvoke*/
    public override void Invoke(ref Actor user)
    {
        if (this.usable && isFinished)
        {
            this.user = user;
            this.isFinished = false;
            InternInvoke(user);
            StartCoroutine(coolDown(cooldownPeriod));
        }
    }

    public override void Invoke(ref Actor user, params object[] args)
    {
        if (this.usable)
        {
            this.user = user;
            this.isFinished = false;
            StartCoroutine(coolDown(cooldownPeriod));
            InternInvoke(easyArgConvert(args));
        }
    }

    /*InternInvoke performs a dodge on user's ActorMovement component*/
    protected override int InternInvoke(params Actor[] args)
    {
        this.hitConnected = false;
        StopCoroutine(sheathe);
        sheathe = SheatheObject();
        interactObject.SetActive(true);
        SpawnObject(args);
        StartCoroutine(sheathe);
        return 0;
    }

    /*This function is meant to be overriiden by other weapon ability derived classes.
By default it performs the players version.*/
    protected virtual void SpawnObject(params Actor[] args)
    {
        interactObject.transform.localPosition = user.faceAnchor.localPosition * objectPositionScale;
    }

    /*SheatheWeapon controls how long the weapon object remains active on screen.
    Use SheathWeapon if there will be no cooldown on the weapon swing.
    This coroutine is being used because we still want some time to elapse 
    before the weapon is set to inactive.*/
    public IEnumerator SheatheObject()
    {
        yield return new WaitForSeconds(duration);
        interactObject.SetActive(false);
        this.isFinished = true;
    }
}
