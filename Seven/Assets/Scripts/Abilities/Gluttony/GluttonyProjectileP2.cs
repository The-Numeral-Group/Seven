using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*GluttonyProjectileP2 derives from gluttyonProjectilep1
Changes how the projectiles are spawned*/
public class GluttonyProjectileP2 : GluttonyProjectileP1
{
    /*Spawns projectiles in the same manner as the phase 1 projectile.
    Differentiates in that they will not be spawned in front of or behind.*/
    protected override IEnumerator SpawnProjectiles(Actor user)
    {
        Debug.Log("p2");
        yield return new WaitForSeconds(projectileDelay);
        int i = 0;
        float dtheta = (2f/numProjectiles) * Mathf.PI; //(360/angle) * (pi/180)
        while(i < numProjectiles)
        {
            Vector2 direction = new Vector2(Mathf.Cos(i*dtheta), Mathf.Sin(i*dtheta));
            if (i * dtheta == Mathf.PI / 2 || i * dtheta == Mathf.PI * 3 / 2)
            {
                i++;
                continue;
            }
            i++;
            GameObject gluttonyProjectile = Instantiate(toInstantiateProjectile, 
                                            user.gameObject.transform.position, Quaternion.identity); 
            ActorMovement currProjectile = gluttonyProjectile.GetComponent<ActorMovement>();
            PROJECTILE_MANAGER.Add(gluttonyProjectile);
            currProjectile.DragActor(direction);
            yield return new WaitForSeconds(projectileSpawnTime/numProjectiles);
        }

        this.isFinished = true;
    }
}
