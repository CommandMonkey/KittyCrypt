using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunOverheatingFire : GunFire<OverheatingGunSettingsObject>
{

    GameSession gamesesion;
    bool isFiring = false;
    protected override void WeaponFire()
    {
        Fire();
        settings.heat += 0.5f;
    }

    protected override void WeaponUpdate()
    {
        settings.heat -= 5f * Time.deltaTime;

        if (settings.heat >= settings.MaxHeat ) 
        {
            Reload(); 
        }

        if (settings.heat < 0) { return; }
    }


    private void Fire()
    {
        RaycastHit2D bulletHit = Physics2D.Raycast(transform.position, transform.right, Mathf.Infinity, ~settings.ignoreLayerMask);
        if (bulletHit)
        {
            runtimeData.bulletsFired++;
            GunFeedbackEffects();
            runtimeData.isFireRateCoolingDown = true;

            GameObject smoke = Instantiate(settings.hitEffect, bulletHit.point, Quaternion.identity);
            RaycastGunLine line = Instantiate(settings.bulletTrail, transform.position, Quaternion.identity).GetComponent<RaycastGunLine>();
            line.Initialize(transform.position, bulletHit.point);

            Destroy(smoke, settings.destroyHitEffectAfter);
            Destroy(line.gameObject, settings.destroyTrailAfter);

            Enemy enemyScript = bulletHit.collider.gameObject.GetComponent<Enemy>();

            if (enemyScript != null)
            {
                enemyScript.TakeDamage(settings.damage);
            }
        }
    }
}
    



