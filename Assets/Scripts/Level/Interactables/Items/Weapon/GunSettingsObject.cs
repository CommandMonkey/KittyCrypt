using UnityEngine;


public class GunSettingsObject : ScriptableObject
{

    //Configurable parameters damage
    [Header("General Options")]
    [SerializeField] internal AudioClip fireAudio;
    [SerializeField] internal AudioClip reloadAudio;
    [SerializeField] internal float audioVolume = 1f;
    [SerializeField] internal float fireRate = 0.5f;
    [SerializeField] internal float reloadTime = 1f;
    [SerializeField] internal float bulletDamage = 1f;
    [SerializeField, Tooltip("Set to 0 for infinite (Except for burst fire)")] internal int bulletsBeforeReload = 5;
    [SerializeField] internal GameObject hitEffect;
    [SerializeField] internal float destroyHitEffectAfter = 1.5f;
    [SerializeField] internal float knockback = 2f;
    [SerializeField] internal LayerMask ignoreLayerMask;
}

[CreateAssetMenu(fileName = "WeaponSettings", menuName = "ScriptableObjects/WeaponSettings/RaycastGunSettings")]
public class RaycastGunSettingsObject : GunSettingsObject
{
    [Header("Raycast Options")]
    [SerializeField] internal GameObject bulletTrail;
    [SerializeField] internal float destroyTrailAfter = .1f;
}

[CreateAssetMenu(fileName = "WeaponSettings", menuName = "ScriptableObjects/WeaponSettings/BurstGunSettings")]
public class BurstGunSettingsObject : GunSettingsObject
{

}

[CreateAssetMenu(fileName = "WeaponSettings", menuName = "ScriptableObjects/WeaponSettings/ProjectileGunSettings")]
public class ProjectileGunSettingsObject : GunSettingsObject
{
    [Header("Projectile Options")]
    [SerializeField] internal GameObject projectile;
    [SerializeField, Range(0f, 180f), Tooltip("Range goes in both directions")] internal float bulletSpreadRange = 1f;
    [SerializeField] internal float bulletSpeed = 5f;
    [SerializeField] internal float projectileVanishAfter = 3f;
}
