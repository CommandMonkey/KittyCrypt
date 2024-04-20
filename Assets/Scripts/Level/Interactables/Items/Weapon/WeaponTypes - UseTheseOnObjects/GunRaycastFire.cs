using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRaycastFire : GunFire
{


    bool RaycastFire()
    {
        if (!gameSession.playerIsShooting)
        {
            StartCoroutine(SetCatAngry());
        }

        // Fire
        bulletHit = Physics2D.Raycast(transform.position, transform.right, Mathf.Infinity, ~settings.ignoreLayerMask);
        if (bulletHit)
        {
            runtimeData.bulletsFired++;
            GunFeedbackEffects();
            runtimeData.isFireRateCoolingDown = true;

            var smoke = Instantiate(settings.hitEffect, bulletHit.point, Quaternion.identity);
            Destroy(smoke, settings.destroyHitEffectAfter);

            var line = Instantiate(settings.bulletTrail, transform.position, Quaternion.identity);
            Destroy(line, settings.destroyTrailAfter);

            Enemy enemyScript = bulletHit.collider.gameObject.GetComponent<Enemy>();

            if (enemyScript != null)
            {
                enemyScript.TakeDamage(settings.bulletDamage);
            }
        }

        // Ammo
        SetAmmoUI();

        return true;
    }
}
