using TMPro;
using UnityEngine;

public class SpawnRoom : Room
{
    [SerializeField] ItemPickupInteractable[] startWeaponPickups;
    [SerializeField] TMP_Text pickUpWeaponText;

    bool popAll = false;

    // Update is called once per frame
    void Update()
    {
        foreach (ItemPickupInteractable weapon in startWeaponPickups)
        {
            if (weapon == null)
            {
                FindObjectOfType<RoomManager>().OpenDoors();
                popAll = true;
                Destroy(this);
                Destroy(pickUpWeaponText);
            }
        }
        if (popAll)
        {
            foreach (ItemPickupInteractable weapon in startWeaponPickups)
            {
                if (weapon != null)
                    weapon.PopAndDie();
            }
        }
    }
}
