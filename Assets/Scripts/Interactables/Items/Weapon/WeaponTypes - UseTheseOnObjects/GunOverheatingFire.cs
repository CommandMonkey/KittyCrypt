using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunOverheatingFire : GunFire<OverheatingGunSettingsObject>
{

    GameSession gamesesion;
    float timeWhenlastFire = 0f;
    protected override void WeaponFire()
    {
        Fire();
        timeWhenlastFire = Time.time;
        settings.heat += 0.5f;
    }

    protected new void Update()
    {
        base.Update();

        if (Time.time - timeWhenlastFire > 1f) settings.heat = Mathf.Max((settings.heat- 8f * Time.deltaTime), 0f);
        Debug.Log(settings.heat);   
        if (settings.heat >= settings.MaxHeat ) 
        {
            Reload(); 
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

    protected override void SetAmmoUI()
    {
        
    }
}
    



