using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunOverheatingFire : GunFire<OverheatingGunSettingsObject>
{

    GameSession gamesesion;

    float heat = 0f;
    bool isFiring = false;
float timePassed = 0f;
    protected override void WeaponUpdate()
    {
        if (gameSession.playerInput.actions["Fire"].IsPressed())
        {
            
            timePassed = Time.time;
            Debug.Log("Time passed: " + timePassed);
            if (timePassed >= settings.heat)
            {
                Reload();
                isFiring = false;
                
            }
            else
            {
              Fire();
            }
        }
        else
        {
            // NOT holding putton
            isFiring = false;
            timePassed = 0f;
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
    



