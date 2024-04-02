using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSettings", menuName = "ScriptableObjects/WeaponSettings")]
public class WeaponSettingsObject : ScriptableObject
{
    public enum WeaponType
    {
        ProjectileFire,
        BurstFire,
        RaycastFire
    }

    //Configurable parameters damage
    [Header("General Options")]
    [SerializeField] internal WeaponType weaponType;
    [SerializeField] internal AudioClip fireAudio;
    [SerializeField] internal AudioClip reloadAudio;
    [SerializeField] internal float fireRate = 0.5f;
    [SerializeField] internal float reloadTime = 1f;
    [SerializeField] internal float damagePerBullet = 1f;
    [SerializeField, Tooltip("Set to 0 for infinite (Except for burst fire)")] internal int bulletsBeforeReload = 5;
    [SerializeField] internal GameObject hitEffect;
    [SerializeField] internal float destroyHitEffectAfter = 1.5f;
    [SerializeField] internal float knockback = 2f;
    [SerializeField] internal LayerMask ignoreLayerMask;

    [Header("Projectile Options")]
    [SerializeField] internal GameObject projectile;
    [SerializeField, Range(0f, 180f), Tooltip("Range goes in both directions")] internal float bulletSpreadRange = 1f;
    [SerializeField] internal float projectileSpeed = 5f;
    [SerializeField] internal float projectileVanishAfter = 3f;

    [Header("Raycast Options")]
    [SerializeField] internal GameObject bulletTrail;
    [SerializeField] internal float destroyTrailAfter = .1f;

}
