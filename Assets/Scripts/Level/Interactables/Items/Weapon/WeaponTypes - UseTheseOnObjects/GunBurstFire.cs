using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBurstFire : GunFire
{
    [SerializeField] private BurstGunSettingsObject burstGunSettings;


    protected override void WeaponFire()
    {
        // Fire
        for (int i = 0; i < settings.bulletsBeforeReload || settings.bulletsBeforeReload == 0; i++)
        {
            ShootBullet();
        }

        GunFeedbackEffects();
        runtimeData.isFireRateCoolingDown = true;
        //Ammo
        SetAmmoBurstUI();

        return true;
    }

    protected override void SetAmmoUI()
    {
        ammoUI.text = (1 - runtimeData.bulletsFired / settings.bulletsBeforeReload).ToString() + "/" +
                      1.ToString();
    }
}
