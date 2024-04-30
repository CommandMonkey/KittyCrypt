using UnityEngine;

public class GunSettingsObject : ScriptableObject
{
    //Configurable parameters damage
    [Header("General Options")]
    [SerializeField] internal float fireRate = 0.5f;
    [SerializeField] internal float reloadTime = 1f;
    [SerializeField, Tooltip("Set to 0 for infinite (Except for burst fire)")] internal int shotsBeforeReload = 5;
    [SerializeField, Range(0f, 180f), Tooltip("Range goes in both directions")] internal float bulletSpreadRange = 1f;

    [Header("Impact")]
    [SerializeField] internal float damage = 1f;
    [SerializeField] internal float playerKnockback = 2f;

    [Header("Effects")]
    [SerializeField] internal AudioClip fireAudio;
    [SerializeField] internal AudioClip reloadAudio;
    [SerializeField, Range(0, 1)] internal float audioVolume = 1f;
    [SerializeField] internal GameObject hitEffect;
    [SerializeField] internal float destroyHitEffectAfter = 1.5f;
}