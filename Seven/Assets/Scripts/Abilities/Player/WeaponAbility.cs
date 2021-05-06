using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*WeaponAbility class
Intantiates a weapon object and performs a weapon attack with that object.*/
public class WeaponAbility : ActorAbilityFunction<Actor, int>
{
    //Prefab to be instantiated as a weapon for w/e actor is using this ability.
    [Tooltip("The weapon prefab this ability will use.")]
    public GameObject startingWeaponObject;
    //The duration of the swing. Used by SheathWeapon coroutine
    [Tooltip("The duration of the weapon swing. AKA how long the animation should take.")]
    public float duration;
    //The scale that determines the local position of a weapon relative to it's owner.
    [Tooltip("The scalar used to position the weapon relative to its' user.")]
    public Vector2 weaponPositionScale = new Vector2(0.1f, 0.1f);
    //A list of damage per each hitbox on the weapon.
    [Tooltip("A list to contain the damage for each hitbox of a weapon. Default value is 0.")]
    public List<int> damagePerHitbox = new List<int>();
    //A pointer to the instatiation of the starting weapon object.
    protected GameObject weaponObject;
    /*A reference to the SheatheWeapon Coroutine. Used to cancel the coroutine if another call to
    this ability is being initiated*/
    protected IEnumerator sheathe;

    /*hitConnected variable is used to check if the current version of the attack connected.
    It used as a flag for weapons with multiple hit boxes. It is intended to be set in weaponability
    as well as the hitbox class.*/
    public bool hitConnected { get; set; }

    //Initializing fields
    protected virtual void Awake()
    {
        this.hitConnected = false;
        sheathe = SheatheWeapon();
    }

    //Initializing mono behavior fields.
    protected virtual void Start()
    {
        weaponObject = Instantiate(startingWeaponObject, this.gameObject.transform);
        //Note From Ram: Keeping the below comment for posterity.
        //the weapon isn't following the game object, I've heard this helps? 
        weaponObject.transform.localPosition = weaponPositionScale;
        /*Unsure if the below line is necessary. Adding it just in case. 
        Making sure the weapon is a child of the actor.*/
        weaponObject.transform.parent = this.gameObject.transform;
        weaponObject.SetActive(false);

        //set damage of hitboxes
        if (damagePerHitbox.Count > 0)
        { 
            SetDamage(damagePerHitbox[0]);
        }
        else
        {
            SetDamage(0);
        }
    }

    /*Similar to ActorAbilityFunction's invoke method
    Passes the users Actor component to InternInvoke*/
    public override void Invoke(ref Actor user)
    {
        //by default, Invoke just does InternInvoke with no arguments
        if(this.usable)
        {
            this.user = user;
            this.isFinished = false;
            StartCoroutine(coolDown(cooldownPeriod));
            InternInvoke(new Actor[0]);
        }
        
    }

    //Invoke which receives an object array as an additional parameter. Meant to be used to pass in
    //targets or other actors.
    public override void Invoke(ref Actor user, params object[] args)
    {
        if(this.usable)
        {
            this.user = user;
            this.isFinished = false;
            StartCoroutine(coolDown(cooldownPeriod));
            InternInvoke(easyArgConvert(args));
        }
    }

    /*The internal invoke for weapon ability activates the weapon prefab attached to the actor object.
    In the future it will likely need to control how the weaponPrefab is swung in some manner.*/
    protected override int InternInvoke(params Actor[] args)
    {
        this.hitConnected = false;
        StopCoroutine(sheathe);
        sheathe = SheatheWeapon();
        SpawnWeapon(args);
        weaponObject.SetActive(true);
        StartCoroutine(sheathe);
        return 0;
    }

    /*This function is meant to be overriiden by other weapon ability derived classes.
    By default it performs the players version.*/
    protected virtual void SpawnWeapon(params Actor[] args)
    {
        //user.mySoundManager.PlaySound("PlayerAttack");
        weaponObject.transform.localPosition = user.faceAnchor.localPosition * weaponPositionScale;
    }
    
    /*SheatheWeapon controls how long the weapon object remains active on screen.
    Use SheathWeapon if there will be no cooldown on the weapon swing.
    This coroutine is being used because we still want some time to elapse 
    before the weapon is set to inactive.*/
    public IEnumerator SheatheWeapon()
    {
        yield return new WaitForSeconds(duration);
        weaponObject.SetActive(false);
        this.isFinished = true;
    }

    //sets the damage of the weapon's hitboxes
    public virtual void SetDamage(int damageArg)
    {
        //Setup the damage for each hitbox in the weapon.
        for (int i = 0; i < weaponObject.transform.childCount; i++)
        {
            //assume the weapon prefab is setup as: object->object with hitbox component.
            WeaponHitbox hb = weaponObject.transform.GetChild(i).GetComponent<WeaponHitbox>();
            if (!hb)
            {
                Debug.LogWarning("WeaponAbility: Current weapon abiltiy is instantiating a weapon without" + 
                        "a WeaponHitbox Component");
                Debug.LogWarning("WeaponAbility: Weaponprefab should be: " +  
                        "spriteObject->childObject(child should contain hibox)");
            }
            else
            {
                if (i < damagePerHitbox.Count)
                {
                    hb.damage = damagePerHitbox[i];
                }
                else
                {
                    hb.damage = 0;
                }
            }
        }
    }

    //adds damage of the weapon's hitboxes. Pass a negative number to subtract
    public void AddDamage(int damageMod)
    {
        //Setup the damage for each hitbox in the weapon.
        for (int i = 0; i < weaponObject.transform.childCount; i++)
        {
            //assume the weapon prefab is setup as: object->object with hitbox component.
            WeaponHitbox hb = weaponObject.transform.GetChild(i).GetComponent<WeaponHitbox>();
            if (!hb)
            {
                Debug.LogWarning("WeaponAbility: Current weapon abiltiy is instantiating a weapon without" + 
                        "a WeaponHitbox Component");
                Debug.LogWarning("WeaponAbility: Weaponprefab should be: " +  
                        "spriteObject->childObject(child should contain hibox)");
            }
            else
            {
                hb.damage += damageMod;
            }
        }
    }

    //Returns the user's transforms, for weapons that change effect depending on
    //the position of the target reletive to the user
    public Transform getUserTransform()
    {
        return user.gameObject.transform;
    }

}
