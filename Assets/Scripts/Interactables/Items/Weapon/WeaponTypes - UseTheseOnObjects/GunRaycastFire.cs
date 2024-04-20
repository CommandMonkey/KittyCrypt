using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRaycastFire : GunFire<RaycastGunSettingsObject>
{
    bool RaycastFire()
    {
        // Fire
        RaycastHit2D bulletHit = Physics2D.Raycast(transform.position, transform.right, Mathf.Infinity, ~settings.ignoreLayerMask);
        if (bulletHit)
        {
            runtimeData.bulletsFired++;
            GunFeedbackEffects();
            runtimeData.isFireRateCoolingDown = true;

            GameObject smoke = Instantiate(settings.hitEffect, bulletHit.point, Quaternion.identity);
            RaycastGunLine line = Instantiate(settings.bulletTrail, transform.position, Quaternion.identity).GetComponent<RaycastGunLine>();

            Destroy(smoke, settings.destroyHitEffectAfter);
            Destroy(line, settings.destroyTrailAfter);

            Enemy enemyScript = bulletHit.collider.gameObject.GetComponent<Enemy>();

            if (enemyScript != null)
            {
                enemyScript.TakeDamage(settings.damage);
            }
        }

        return true;
    }
}
