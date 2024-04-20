using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunProjectileFire : GunFire
{


    void ProjectileFire()
    {
        if (settings.weaponType != GunSettingsObject.WeaponType.ProjectileFire)
        {
            return;
        }

        // Fire
        ShootBullet();
        GunFeedbackEffects();
        runtimeData.isFireRateCoolingDown = true;


        // Ammo
        SetAmmoUI();

    }
}
