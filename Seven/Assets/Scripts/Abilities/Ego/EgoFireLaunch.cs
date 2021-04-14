using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EgoFireLaunch : ProjectileAbility
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("The closest the user should appear to its target after a teleport.")]
    public float teleMinDist = 1.5f;

    [Tooltip("The farthest the user should appear from its target after a teleport.")]
    public float teleMaxDist = 7f;

    [Tooltip("The delay between teleport-firewall combos.")]
    public float attackDelay = 0.5f;

    [Tooltip("The number of firewalls launched by this ability (minimum 1).")]
    public int attackCount = 4;

    //METHODS--------------------------------------------------------------------------------------
    /*Launch the projectile, kind of. Because this ability is timing dependent, it is handled with a
    coroutine*/
    protected override int InternInvoke(params Vector2[] args)
    {
        usable = false;
        StartCoroutine(FireInvokation(args[0]));
        return 1;
    }

    //handles the actual launching of the firewalls
    IEnumerator FireInvokation(Vector2 targetPoint)
    {
        //Step 0: Force launchMode to be DIRECTION
        //no matter what, these firewalls will be launched in directions
        this.launchMode = LAUNCH_MODE.DIRECTION;

        //Step 1: Launch the first fire wall
        this.projObj.GetComponent<BasicProjectile>().Launch(targetPoint, launchMode);

        //Step 2: Do the rest of the ability lol
        for(int i = 0; i < attackCount - 1; ++i)
        {
            //Step 2.1: Wait a little bit
            yield return new WaitForSeconds(attackDelay);

            //Step 2.2: Teleport to a random location near the target.
            var dest = (Vector3)targetPoint + new Vector3(
                Random.Range(teleMinDist, teleMaxDist) * (Random.Range(0, 2) * 2 - 1),
                Random.Range(teleMinDist, teleMaxDist) * (Random.Range(0, 2) * 2 - 1),
                0f
            );
            Ego2Movement.EgoTeleport(dest, user);

            //Step 2.3: Make a new projectile object
            this.projObj = InstantiateProjectile(projectile, user.faceAnchor, this.projectileScale);

            //Step 2.4: Launch that Projectile
            this.projObj.GetComponent<BasicProjectile>().Launch(targetPoint, this.launchMode);

            //Repeat
        }

        //Step 3: Turn on the cooldown. Attack is done.
        StartCoroutine(coolDown(cooldownPeriod));

        isFinished = true;
    }
}
