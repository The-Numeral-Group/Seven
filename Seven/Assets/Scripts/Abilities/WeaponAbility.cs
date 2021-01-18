using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Though weapon attacks are being treated as an ability, 
I assume they would not be invoked by the ability Initiator 
seeing as weapon attacks are engaged by OnAttack. 
But that can be changed since the whole weapon sequence is initiated by the OnAttack call.
Note: The template paramters Gameobject and int were chosen arbitrarily.*/
public class WeaponAbility : ActorAbilityFunction<Actor, int>
{
    public GameObject startingWeaponObject;
    public float duration;
    public Vector2 weaponPositionScale = new Vector2(0.1f, 0.1f);
    public List<int> damagePerHitbox = new List<int>();
    private GameObject weaponObject;
    //private Actor actor;

    //hitConnected variable is used to check if the current version of the attack connected.
    //It used as a flag for weapons with multiple hit boxes.
    public bool hitConnected { get; set; }

    protected void Start()
    {
        this.hitConnected = false;
        //this.actor = this.gameObject.GetComponent<Actor>();
        this.weaponObject = Instantiate(this.startingWeaponObject, this.gameObject.transform) as GameObject;
        //Note From Ram: Keeping the below comment from posterity.
        //the weapon isn't following the game object, I've heard this helps? 
        this.weaponObject.transform.localPosition = weaponPositionScale;
        //Unsure if the below line is necessary but adding it just in case. Making sure the weapon is a child of the actor.
        this.weaponObject.transform.parent = this.gameObject.transform;
        this.weaponObject.SetActive(false);

        //Setup the damage for each hitbox in the weapon.
        for (int i = 0; i < this.weaponObject.transform.childCount; i++)
        {
            WeaponHitbox hb = this.weaponObject.transform.GetChild(i).GetComponent<WeaponHitbox>();
            if (!hb)
            {
                Debug.Log("Current weapon abiltiy is instantiating a weapon without a WeaponHitbox Component");
            }
            else
            {
                if (i < this.damagePerHitbox.Count)
                {
                    hb.damage = this.damagePerHitbox[i];
                }
                else
                {
                    hb.damage = 0;
                }
            }
        }
    }

    public override void Invoke(ref Actor user)
    {
        //by default, Invoke just does InternInvoke with no arguments
        if(usable)
        {
            isFinished = false;
            StartCoroutine(coolDown(cooldownPeriod));
            InternInvoke(user);
        }
        
    }

    //The internal invoke for weapon ability activates the weapon prefab attached to the actor object.
    protected override int InternInvoke(params Actor[] args)
    {
        this.weaponObject.SetActive(true);
        this.hitConnected = false;
        this.weaponObject.transform.localPosition = args[0].faceAnchor.position * weaponPositionScale;
        StartCoroutine(SheathWeapon()); //Remove this call if we use the overriden coolDown coroutine.
        return 0;
    }

    //if we want the actual cooldown on the weapon swing to reflect if the weapon is active or inactive.
    /*public override IEnumerator coolDown(float cooldownDuration)
    {
        usable = false;
        yield return new WaitForSeconds(cooldownDuration);
        this.weaponObject.SetActive(false);
        usable = true;
    }*/

    //Use SheathWeapon if there will be no cooldown on the weapon swing.
    //This coroutine is being used because we still want some time to elapse before the weapon is set to inactive.
    public IEnumerator SheathWeapon()
    {
        yield return new WaitForSeconds(this.duration);
        this.weaponObject.SetActive(false);
        isFinished = true;
    }

}
