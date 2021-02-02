using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrideCharge : ActorAbilityFunction<Actor, IEnumerator>
{
    //FIELDS---------------------------------------------------------------------------------------
    [Header("I (Thomas) intend to add a thing for pre-charge animations, but I don't know how" + 
        " animations work so I'm gonna do it later lol.")]
        
    [Header("Charge Properties")]
    [Tooltip("How fast the charge is.")]
    public float chargeSpeed = 25.0f;

    [Tooltip("How much damage the charge should do, independent of the followup punch.")]
    public float chargeDamage = 0.0f;

    [Tooltip("The ability/attack/whatever to followup with after the charge.")]
    public ActorAbility followUp;

    [Header("Charge Duration")]
    [Tooltip("The maximum distance this charge should travel before it stops.")]
    public float maxChargeDistance = 200.0f;

    [Tooltip("The maximum amount of time the charge should travel for.")]
    public float maxChargeTime = 10.0f;

    //an internal reference to the player, PrideCharge's only target.
    //[Tooltip("A target Actor to charge at.")]
    //public Actor target;

    //The actorMovement that's going to be controlling the charge
    ActorMovement userMover;

    //The charge's starting point (for determining how far it's gone)
    private Vector2 origin;

    //The user's original speed (to reset back to after the charge)
    float originalSpeed;
    
    //METHODS--------------------------------------------------------------------------------------
    /*Times this ability's cooldown. */
    public override IEnumerator coolDown(float cooldownDuration)
    {
        usable = false;
        yield return new WaitForSeconds(cooldownDuration);
        usable = true;
    }

    /*InternInvokes the ability as normal, but also grab's the user's actorMovement, if any.*/
    public override void Invoke(ref Actor user)
    {
        if(usable)
        {
            var target = GameObject.FindWithTag("Player").GetComponent<Actor>();
            if(target == user)
            {
                Debug.LogWarning("PrideCharge: ability has no supported no-argument invoke" +
                    " when the user is the player.");
                return;
            }
            isFinished = false;
            userMover = user.myMovement;
            StartCoroutine(InternInvoke(target));
            //the cooldown is started in InternInvoke
        }
        
    }

    public override void Invoke(ref Actor user, params object[] args)
    {
        if(usable)
        {
            isFinished = false;
            userMover = user.myMovement;
            if(args[0] is Actor && args[0] as Actor != user)
            {
                StartCoroutine(InternInvoke((Actor)args[0]));
            }
            else
            {
                Debug.LogWarning("PrideCharge: ability cannot be targeted at its user");
                isFinished = true;
                return;
                //StartCoroutine(InternInvoke(target));
            }
            //the cooldown is started in InternInvoke
        }
    }

    /*InternInvoke the actual ability. Wraps two coroutines so the ability and its cooldown
    can be effectively chained, one after the other.*/
    protected override IEnumerator InternInvoke(params Actor[] args)
    {
        usable = false;
        origin = userMover.gameObject.transform.position;
        yield return StartCoroutine(AbilityFunc(args[0]));
        yield return StartCoroutine(coolDown(cooldownPeriod));
    }

    /*The actual charge. This returns an IEnumerator so it can be called as a 
    coroutine to help run the charge over time.*/
    IEnumerator AbilityFunc(Actor arg)
    {
        //set the user's speed for the charge
        originalSpeed = userMover.speed;
        userMover.speed = chargeSpeed;

        //get colliders for convinience
        var userCollider = userMover.gameObject.GetComponent<Collider2D>();
        var targetCollider = arg.gameObject.GetComponent<Collider2D>();

        //set timer for charge
        var chargeTimer = 0.0f;

        //while the charge has more time and more distance to go...
        while(chargeTimer < maxChargeTime && 
            Mathf.Abs(Vector2.Distance(userMover.transform.position, origin)) < maxChargeDistance)
        {
            //put off operation of this function to the next frame...
            yield return null;

            //get both the user and the target's positions
            var chargePos = userMover.gameObject.transform.position;
            var targetPos = arg.gameObject.transform.position;

            //calculate the normalized direction from user to target
            var mD = (chargePos - targetPos).normalized;

            //take one step forward
            userMover.MoveActor(mD);

            //if the user and target alaunch the shockwave
            if(userCollider.IsTouching(targetCollider))
            {
                //arbitrarily max out the charge timer to auto-end the charge
                chargeTimer = maxChargeTime * 2;
            }
            else
            {
                //if not, just increase it by normal time
                chargeTimer += Time.deltaTime;
            }
        }

        //if the target and user are still touching...
        if(userCollider.IsTouching(targetCollider))
        {
            //hurt them.
            arg.myHealth.takeDamage(chargeDamage);

            //var userActor = userMover.gameObject.GetComponent<Actor>();
            followUp?.Invoke(ref arg);
        }
    }
}
