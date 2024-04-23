using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileGunSettings", menuName = "ScriptableObjects/WeaponSettings/ProjectileGunSettings")]
public class ProjectileGunSettingsObject : GunSettingsObject
{
    [Header("ProjectileFire Options")]
    [SerializeField] internal GameObject projectile;
    [SerializeField] internal float bulletSpeed = 5f;
}
