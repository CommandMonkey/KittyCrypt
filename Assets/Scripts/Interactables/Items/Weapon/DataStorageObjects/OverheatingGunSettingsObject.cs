using UnityEngine;

[CreateAssetMenu(fileName = "OverheatingGunSettings", menuName = "ScriptableObjects/WeaponSettings/OverheatingGunSettings")]
public class OverheatingGunSettingsObject : RaycastGunSettingsObject
{
    [SerializeField] internal float heat = 10;
}
