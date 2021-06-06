using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathFBFire : ActorMovement
{

    // How long the fireball will take to drop
    public float duration;

    public float delayCollide;

    private float delaySpeedMultiplier;

    protected override void Start()
    {
        base.Start();

        delaySpeedMultiplier = WrathP2Actor.abilitySpeedMultiplier;

        if(delaySpeedMultiplier == 1.5)
        {
            delaySpeedMultiplier = 1.42f;
        }

        StartCoroutine(LockActorMovement(Mathf.Infinity));
        StartCoroutine(flyProjectile());
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private IEnumerator flyProjectile()
    {
        Vector2 parentPos = this.transform.parent.gameObject.transform.position;
        parentPos.y += 2.5f;
        float distance = Vector2.Distance(this.transform.position, parentPos);
        float flyingSpeed = distance / (this.duration / delaySpeedMultiplier);

        this.DragActor(new Vector2(0.0f, -1.0f) * flyingSpeed);

        yield return new WaitForSeconds(this.duration / delaySpeedMultiplier);

        this.DragActor(Vector2.zero);

        // Delay before turning on Collider
        //yield return new WaitForSeconds(this.delayCollide / delaySpeedMultiplier);

        // turn on collider
        this.GetComponentInParent<PolygonCollider2D>().enabled = true;
    }

}
