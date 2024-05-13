using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRailgunFire : GunFire<PiercingRaycastGunSettingsObject>
{
    bool isFiring = false;
    float startFireTime;
    protected override void WeaponFire()
    {
        if (!isFiring)
        {
            startFireTime = Time.time;
            isFiring = true;
        }
    }

    protected override void WeaponUpdate()
    {
        if (gameSession.playerInput.actions["Fire"].IsPressed())
        {
            float timePassed = Time.time - startFireTime;
            Debug.Log("Time passed: " + timePassed);
            if (timePassed >= settings.chargeUpTime)
            {
                Fire();
                isFiring = false; 
            }
        }
        else
        {
            // NOT holding putton
            isFiring = false;
        }
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
