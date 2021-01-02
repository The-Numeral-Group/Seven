using UnityEngine;

[RequireComponent(typeof(ActorHealth))]
[RequireComponent(typeof(ActorEffectHandler))]
[RequireComponent(typeof(ActorMovement))]
[RequireComponent(typeof(ActorAbilityInitiator))]
public class Actor : MonoBehaviour
{
    //this is how autoproperties work in C#
    public ActorAbilityInitiator myAbilityInitiator{ get; private set; }
    public ActorEffectHandler myEffectHandler{ get; private set; }
    public ActorHealth myHealth{ get; private set; }
    public ActorMovement myMovement{ get; private set; }
    //turned this into a property 'cause why not?
    public Transform faceAnchor{ get; private set; }

    void Awake()
    {
        
    }

    void Start()
    {
        //all of these need to be in Start to make sure they exist
        faceAnchor = this.gameObject.transform.Find("FaceAnchor").transform;
        this.myAbilityInitiator = this.gameObject.GetComponent<ActorAbilityInitiator>();
        this.myEffectHandler = this.gameObject.GetComponent<ActorEffectHandler>();
        this.myHealth = this.gameObject.GetComponent<ActorHealth>();
        this.myMovement = this.gameObject.GetComponent<ActorMovement>();    
    }

    public virtual void DoActorDamageEffect()
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

    public void DoActorUpdateFacing()
    {
        //Do something that updates the transform of the faceanchor
        //Not sure what this will do in the long run
    }
}
