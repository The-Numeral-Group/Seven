using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GluttonyProjectileP2 : GluttonyProjectileP1
{
    protected override IEnumerator SpawnProjectiles(Actor user)
    {
        yield return new WaitForSeconds(projectileDelay);
        int i = 0;
        float dtheta = (2f/numProjectiles) * Mathf.PI; //(360/angle) * (pi/180)
        while(i < numProjectiles)
        {
            if (i * dtheta == Mathf.PI / 2 || i * dtheta == Mathf.PI * 3 / 2)
            {
                continue;
            }
            Vector2 direction = new Vector2(Mathf.Cos(i*dtheta), Mathf.Sin(i*dtheta));
            GameObject gluttonyProjectile = Instantiate(toInstantiateProjectile, 
                                            user.gameObject.transform.position, Quaternion.identity);
            ActorMovement currProjectile = gluttonyProjectile.GetComponent<ActorMovement>();
            PROJECTILE_MANAGER.Add(gluttonyProjectile);
            currProjectile.DragActor(direction);
            yield return new WaitForSeconds(projectileSpawnTime/numProjectiles);
            i++;
        }

        this.isFinished = true;
    }
}
