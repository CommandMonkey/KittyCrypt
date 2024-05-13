using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBurstFire : GunFire<BurstGunSettingsObject>
{
    protected override void WeaponFire()
    {
        // Fire
        for (int i = 0; i < settings.bulletsPerShot || settings.bulletsPerShot == 0; i++)
        {
            ShootBullet(settings.projectile, settings.bulletSpeed);
        }

        GunFeedbackEffects();
        runtimeData.isFireRateCoolingDown = true;
        runtimeData.bulletsFired++;
        //Ammo
        SetAmmoUI();
    }
}
