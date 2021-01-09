using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int damage = 0;
    void Start()
    {
        //set damage for individual hitboxes. This might vary depending on the weapon
        //such as a sword that hits harder at the very tip
        for(int childIndex = 0; childIndex < this.gameObject.transform.childCount; ++childIndex){
            var childTransform = this.gameObject.transform.GetChild(childIndex);
            childTransform.gameObject.GetComponent<WeaponHitbox>().damage = damage;
        }  
    }
}
