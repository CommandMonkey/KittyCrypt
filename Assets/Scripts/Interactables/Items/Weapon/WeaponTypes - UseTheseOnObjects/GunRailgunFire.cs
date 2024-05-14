using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GunRailgunFire : GunFire<PiercingRaycastGunSettingsObject>
{

    bool hasFired = false;
    bool isFiring = false;
    float startFireTime;

    Light2D nuzzleLight;

    protected override void WeaponFire()
    {
        if (!isFiring)
        {
            startFireTime = Time.time;
            isFiring = true;
        }
    }

    protected override void WeaponStart()
    {
        nuzzleLight = GetComponent<Light2D>();
    }
    protected override void WeaponUpdate()
    {
        if (gameSession.playerInput.actions["Fire"].IsPressed() && hasFired == false)
        {
            float timePassed = Time.time - startFireTime;
            Debug.Log("Time passed: " + timePassed);
            nuzzleLight.enabled = true;
            nuzzleLight.intensity = timePassed * 75;
            if (timePassed >= settings.chargeUpTime)
            {
                nuzzleLight.intensity = 50f;
                Fire();
                isFiring = false;
                hasFired = true;
                StartCoroutine(ShootCoolDownRoutine());
            }
        }
        else
        {
            // NOT holding putton
            isFiring = false;
            nuzzleLight.enabled = false;
        }
    }

    IEnumerator ShootCoolDownRoutine()
    {
        yield return new WaitForSeconds(settings.reloadTime);
        hasFired = false;
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
