using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Abstract class for objects that are interactable
Make sure the collider attached to this object is set to be a trigger.*/
[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : MonoBehaviour
{
    //A static refernce to the current potential interactable. Utilized by the player
    public static Interactable POTENTIAL_INTERACTABLE;
    //Gameobject/sprite that will show if an object can be interacted with
    [SerializeField]
    [Tooltip("The prefab that should appear to indicate this object can be interacted with.")]
    protected GameObject interactIndicatorPrefab;
    //Reference to the actual interactable indicator.
    protected GameObject interactIndicator;
    //How much to offset the indicator when it appears.
    [Tooltip("How much to offset the indicator when it appears.")]
    public Vector2 indicatorOffset = new Vector2(0, 5);
    //Reference to the objects collider;
    public Collider2D colliderInfo {get; protected set;}
    // Start is called before the first frame update

    //Setup in class references
    protected virtual void Awake()
    {
        colliderInfo = GetComponent<Collider2D>();
        SetupIndicator();
    }

    /*This is the function that the player will call when they try to interact with an interactable
    This function is abstract because each type of interactable script should create its own version
    of this given the result the user would want to happen as a result of the interaction.*/

    public abstract void OnInteract();

    //What happens when the player gets in range of this object
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            ShowIndicator(true);
            SetPotentialInteractable(true, other.gameObject);
        }
    }

    //What happens when the player exits the range of this object
    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            ShowIndicator(false);
            SetPotentialInteractable(false, other.gameObject);
        }
    }

    /*On trigger stay used to choose between multiple interactable objects within range of the player.
    performs selection based on distance.*/
    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (POTENTIAL_INTERACTABLE && POTENTIAL_INTERACTABLE != this)
            {
                float myDistanceToPlayer = Vector2.Distance(other.transform.position, this.transform.position);
                float closestPotentialDistanceToPlayer = Vector2.Distance(other.transform.position, 
                                                            POTENTIAL_INTERACTABLE.transform.position);
                if (myDistanceToPlayer < closestPotentialDistanceToPlayer)
                {
                    ShowIndicator(true);
                    SetPotentialInteractable(true, other.gameObject);
                }
                else
                {
                    ShowIndicator(false);
                }
            }
            else if (!POTENTIAL_INTERACTABLE)
            {
                ShowIndicator(true);
                SetPotentialInteractable(true, other.gameObject);
            }
        }
    }

    //Instantiate the prefab and set its parent to the interactable object.
    protected virtual void SetupIndicator()
    {
        if (!interactIndicatorPrefab)
        {
            Debug.LogWarning("Interactable: " + this.gameObject.name + " does not have their " + 
            "indicator prefab setup.");
            interactIndicator = new GameObject("Interact Indicator");
        }
        else
        {
            interactIndicator = Instantiate(interactIndicatorPrefab, this.transform);
        }
        interactIndicator.transform.parent = transform;
        interactIndicator.transform.localPosition = new Vector3(indicatorOffset.x,
                                                        indicatorOffset.y, transform.position.z);
        interactIndicator.SetActive(false);
    }

    //enables/disables the indicator prefab based on the boolean value passed in
    protected virtual void ShowIndicator(bool value)
    {
        interactIndicator.SetActive(value);
    }

    /*Called by the ontriggerenter function. If the player is in range the static reference will call
    on this object. Informs menu manager for where to place the interaction prompt using the player.*/
    protected virtual void SetPotentialInteractable(bool value, GameObject t)
    {
        if (value)
        {
            Interactable.POTENTIAL_INTERACTABLE = this;
            MenuManager.INTERACT_MENU.target = t;
            MenuManager.INTERACT_MENU.uiElementOffset = new Vector2(1, -1);
            MenuManager.INTERACT_MENU.Show();
        }
        else if (Interactable.POTENTIAL_INTERACTABLE == this) 
        {
            //implicitly this is only reached if value is false as well.
            Interactable.POTENTIAL_INTERACTABLE = null;
            MenuManager.INTERACT_MENU.target = null;
            MenuManager.INTERACT_MENU.Hide();
        }
    }
}
