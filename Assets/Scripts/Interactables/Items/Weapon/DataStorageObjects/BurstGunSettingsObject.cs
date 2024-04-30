using UnityEngine;

[CreateAssetMenu(fileName = "BurstGunSettings", menuName = "ScriptableObjects/WeaponSettings/BurstGunSettings")]
public class BurstGunSettingsObject : GunSettingsObject
{
    [Header("BurstFire Options")]
    [SerializeField] internal int bulletsPerShot;
    [SerializeField] internal GameObject projectile;
    [SerializeField] internal float bulletSpeed = 5f;
}
