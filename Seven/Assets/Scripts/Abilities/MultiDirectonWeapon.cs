using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiDirectonWeapon : MonoBehaviour
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("Whether this gameObjecct will try to figure out which hitbox to use based on the" + 
        " local position of its parent or not.")]
    public bool assumeDirectionFromParent;

    [Tooltip("The direction of the hitbox that needs to be used when this weapon is swung.")]
    public Vector2 inputDir = Vector2.up;

    [Tooltip("All of the subhitboxes this weapon uses.")]
    public GameObject northBox;
    public GameObject southBox;
    public GameObject eastBox;
    public GameObject westBox;
    public GameObject northEastBox;
    public GameObject southEastBox;
    public GameObject northWestBox;
    public GameObject southWestBox;
    
    //METHODS--------------------------------------------------------------------------------------
    // Called when this gameObject goes from inactive to active
    void OnEnable()
    {
        DeactivateAll();

        /*WeaponAbility wp = this.gameObject.GetComponentInParent<WeaponAbility>(true);
        if(!wp)
        {
            Debug.LogError("MultiDirectionWeapon: Can't find an ability to get orientation from!");
        }*/

        GameObject childBox;
        if(assumeDirectionFromParent)
        {
            //var userFace = wp.getUserTransform().Get
            childBox = DecodeChild(this.gameObject.transform.localPosition.normalized);
        }
        else
        {
            childBox = DecodeChild(inputDir.normalized);
        }

        childBox.SetActive(true);
    }

    //Deactivates all of this object's children
    void DeactivateAll()
    {
        for(int i = 0; i < this.gameObject.transform.childCount; ++i)
        {
            this.gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    //Returns which part of the hitbox should become active in this moment
    GameObject DecodeChild(Vector2 vector)
    {
        //It's not pretty, but it will do...
        GameObject outbox;
        Vector2 vec = vector.normalized;
        if(vec == Vector2.up)
        {
            outbox = northBox;
        }
        else if(vec == Vector2.down)
        {
            outbox = southBox;
        }
        else if(vec == Vector2.left)
        {
            outbox = westBox;
        }
        else if(vec == Vector2.right)
        {
            outbox = eastBox;
        }
        else if(vec.x > 0f && vec.y > 0f) //northeast
        {
            outbox = northEastBox;
        }
        else if(vec.x > 0f && vec.y < 0f) //southeast
        {
            outbox = southEastBox;
        }
        else if(vec.x < 0f && vec.y > 0f) //northWest
        {
            outbox = northWestBox;
        }
        else if(vec.x < 0f && vec.y < 0f) //southwest
        {
            outbox = southWestBox;
        }
        else
        {
            outbox = northBox;
        }

        return outbox;
    }
}
