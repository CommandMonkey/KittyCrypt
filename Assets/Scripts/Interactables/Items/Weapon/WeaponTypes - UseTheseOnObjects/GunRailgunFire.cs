using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class GunRailgunFire : GunFire<PiercingRaycastGunSettingsObject>
{

    bool hasFired = false;
    bool isFiring = false;
    float startFireTime;

    protected override void WeaponFire()
    {
        if (!isFiring && !runtimeData.isReloading)
        {
            startFireTime = Time.time;
            isFiring = true;
        }
    }

    protected new void Start()
    {
        base.Start();

        nuzzleLight = GetComponent<Light2D>();
    }
    protected new void Update()
    {
        base.Update();

        if (gameSession.playerInput.actions["Fire"].IsPressed() && hasFired == false && startFireTime != 0f)
        {
            float timePassed = Time.time - startFireTime;
            nuzzleLight.enabled = true;
            nuzzleLight.intensity = timePassed * 75;
            if (timePassed >= settings.chargeUpTime)
            {
                nuzzleLight.intensity = 50f;
                PiercingRaycastFire();
                isFiring = false;
                hasFired = true;
                startFireTime = 0f;
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

    private void PiercingRaycastFire()
    {
        RaycastHit2D bulletHit = Physics2D.Raycast(transform.position, transform.right, Mathf.Infinity, settings.obsticleLayer);
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

            Vector2 toHit = bulletHit.point - (Vector2)transform.position;
            List<Collider2D> enemyColliders = new List<Collider2D>();
            int unitlenghtsTraveled = 0;

            while ((toHit.normalized * unitlenghtsTraveled).magnitude < toHit.magnitude)
            {
                enemyColliders.AddRange(Physics2D.OverlapCircleAll((toHit.normalized * unitlenghtsTraveled) + (Vector2)transform.position, 1f, settings.enemyLayer));
                unitlenghtsTraveled++;
            }

            foreach (Collider2D collider in enemyColliders)
            {
                Enemy enemyScript = collider.gameObject.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.TakeDamage(settings.damage);
                }
            }
        }
    }

    public override void DeActivate()
    {
        base.DeActivate();
        hasFired = false;
        isFiring = false;
    }
}
