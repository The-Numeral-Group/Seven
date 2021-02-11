using UnityEngine;

[RequireComponent(typeof(ActorHealth))]
[RequireComponent(typeof(ActorEffectHandler))]
[RequireComponent(typeof(ActorMovement))]
[RequireComponent(typeof(ActorAbilityInitiator))]
[RequireComponent(typeof(ActorAnimationHandler))]
public class Actor : MonoBehaviour
{
    //this is how autoproperties work in C#
    public ActorAbilityInitiator myAbilityInitiator{ get; protected set; }
    public ActorEffectHandler myEffectHandler{ get; protected set; }
    public ActorHealth myHealth{ get; protected set; }
    public ActorMovement myMovement{ get; protected set; }
    public ActorAnimationHandler myAnimationHandler { get; protected set; }
    //turned this into a property 'cause why not?
    public Transform faceAnchor{ get; protected set; }

    protected virtual void Start()
    {
        //all of these need to be in Start to make sure they exist

        //Alternatively I (Ram) think we can create the face anchor when the actor gets instatiated into the scene rather than having the face anchor already exist in the prefab.
        //pros and cons to both sides, so whatever you all prefer between the two I am down for.
        this.faceAnchor = this.gameObject.transform.Find("FaceAnchor");
        ///DEBUG
        if(!this.faceAnchor)
        {
            GameObject facingDirection = new GameObject("FaceAnchor");
            facingDirection.transform.parent = this.gameObject.transform;
            facingDirection.transform.localPosition = new Vector3(0,0,0);
            this.faceAnchor = facingDirection.gameObject.transform;
        }
        ///DEBUG
        
        this.myAbilityInitiator = this.gameObject.GetComponent<ActorAbilityInitiator>();
        this.myEffectHandler = this.gameObject.GetComponent<ActorEffectHandler>();
        this.myHealth = this.gameObject.GetComponent<ActorHealth>();
        this.myMovement = this.gameObject.GetComponent<ActorMovement>();
        this.myAnimationHandler = this.gameObject.GetComponent<ActorAnimationHandler>();
    }

    public virtual void DoActorDamageEffect(float damage)
    {
        //Do anything that should happen on taking damage
        //no effect by default
    }

    public virtual void DoActorDeath()
    {
        //Do anything that should happen on death
        //destroys the gameobject by default
        Destroy(this.gameObject);
    }

    public void DoActorUpdateFacing(Vector2 newDirection)
    {
        //Do something that updates the transform of the faceanchor
        //Not sure what this will do in the long run
        if (newDirection != Vector2.zero)
        {
            faceAnchor.localPosition = newDirection;
        }
    }
}
