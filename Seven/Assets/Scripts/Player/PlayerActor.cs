using UnityEngine;

public class PlayerActor : Actor
{
    //The following needs to be moved over to the weapon ability script;
    /*public GameObject startingWeaponObject;
    private GameObject weaponObject;
    private Vector2 weaponPositionScale = new Vector2(0.1f, 0.1f);*/
    protected override void Start()
    {
        base.Start();
        //The following needs to be moved over to the weapon ability script.
        /*weaponObject = Instantiate(startingWeaponObject, this.gameObject.transform.position, Quaternion.identity);
        //Note From Ram: Keeping the below comment from posterity.
        //the weapon isn't following the game object, I've heard this helps? 
        weaponObject.transform.localPosition = weaponPositionScale;
        weaponObject.SetActive(false);*/
    }
}
