using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Though weapon attacks are being treated as an ability, 
I assume they would not be invoked by the ability Initiator 
seeing as weapon attacks are engaged by OnAttack. 
But that can be changed since the whole weapon sequence is initiated by the OnAttack call.
Note: The template paramters Gameobject and int were chosen arbitrarily.*/
public class WeaponAbility : ActorAbilityFunction<GameObject, int>
{
    public GameObject startingWeaponObject;
    private GameObject weaponObject;
    private Vector2 weaponPositionScale = new Vector2(0.1f, 0.1f);
    private Actor actor;

    //hitConnected variable is used to check if the current version of the attack connected.
    //It used as a flag for weapons with multiple hit boxes.
    [HideInInspector]public bool hitConnected;

    private void Start()
    {
        this.hitConnected = false;
        this.actor = this.gameObject.GetComponent<Actor>();
        this.weaponObject = Instantiate(this.startingWeaponObject, this.gameObject.transform) as GameObject;
        //Note From Ram: Keeping the below comment from posterity.
        //the weapon isn't following the game object, I've heard this helps? 
        this.weaponObject.transform.localPosition = weaponPositionScale;
        //Unsure if the below line is necessary but adding it just in case. Making sure the weapon is a child of the actor.
        this.weaponObject.transform.parent = this.gameObject.transform;
        this.weaponObject.SetActive(false);
    }

    //The internal invoke for weapon ability activates the weapon prefab attached to the actor object.
    protected override int InternInvoke(GameObject[] args)
    {
        this.weaponObject.SetActive(true);
        this.hitConnected = false;
        this.weaponObject.transform.localPosition = actor.faceAnchor.position * weaponPositionScale;
        StartCoroutine(SheathWeapon()); //Remove this call if we use the overriden coolDown coroutine.
        return 0;
    }

    /*OnAttack is the function the input system will call to perform an attack. 
    If this ends up being moved out of this class
    just uncomment the send message line.*/
    /*public void OnAttack()
    {
        //this.gameObject.SendMessage("DoWeaponAttack");
        this.DoWeaponAttack();
    }*/

    /*DoWeaponAttack is used to receive the message to call the invoke function of the ability class.
    If we call invoke directly from ability initiator or this isn't needed.
    I (Ram) added it because I am not sure if if the weapon ability would be called from ability initiator.
    From the players pov the input system will initiate the call using a different call to what would normally engage abilities.
    I thought about the possibility of giving the enemy a weapon ability, then in that case having this function helps deal with both cases.
    This comment is poorly written because it is late and I am incoherent. Apologies*/

    private void DoWeaponAttack()
    {
        //Though we call base invoke it can still reference the derived cooldown coroutine posted below if we uncomment it.
        base.Invoke(ref this.actor);
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
        yield return new WaitForSeconds(1f);
        this.weaponObject.SetActive(false);
    }

}
