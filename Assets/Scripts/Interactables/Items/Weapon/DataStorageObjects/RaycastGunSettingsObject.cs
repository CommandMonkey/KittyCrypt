using UnityEngine;

[CreateAssetMenu(fileName = "RaycastGunSettings", menuName = "ScriptableObjects/WeaponSettings/RaycastGunSettings")]
public class RaycastGunSettingsObject : GunSettingsObject
{
    [Header("RaycastFire Options")]
    [SerializeField] internal GameObject bulletTrail;
    [SerializeField] internal float destroyTrailAfter = .1f;
    [SerializeField] internal LayerMask ignoreLayerMask;
}
