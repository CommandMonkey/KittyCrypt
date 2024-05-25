using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunProjectileFire : GunFire<ProjectileGunSettingsObject>
{
    protected override void WeaponFire()
    {
        Debug.Log("Projectile Fire");
        // Fire
        ShootBullet(settings.projectile, settings.bulletSpeed);
        GunFeedbackEffects();
        runtimeData.isFireRateCoolingDown = true;

        runtimeData.bulletsFired++;
        // Ammo
        SetAmmoUI();
    }
}
