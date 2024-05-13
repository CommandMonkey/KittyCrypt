using UnityEngine;

[CreateAssetMenu(fileName = "PiercingRaycastGunSettingsObject", menuName = "ScriptableObjects/WeaponSettings/PiercingRaycastGunSettingsObject")]
public class PiercingRaycastGunSettingsObject : GunSettingsObject
{
    [Header("RaycastFire Options")]
    [SerializeField] internal float chargeUpTime = 3;

    [SerializeField] internal GameObject bulletTrail;
    [SerializeField] internal float destroyTrailAfter = .1f;
    [SerializeField] internal LayerMask ignoreLayerMask;
}
