using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SpawnRoom : Room
{
    [SerializeField] Transform[] startWeaponPoints;
    [SerializeField] GameObject itemPickupInteractable;
    [SerializeField] TMP_Text pickUpWeaponText;

    List<ItemPickupInteractable> startWeaponPickups = new List<ItemPickupInteractable>();
    List<GameObject> itemsSpawned = new List<GameObject>();

    bool popAll = false;

    protected override void RoomStart()
    {
        foreach (Transform t in startWeaponPoints)
        {
            GameObject item = null;
            while (item == null || itemsSpawned.Contains(item))
            {
                item = gameSession.levelSettings.GetRandomSpawnRoomItem();
            }
            itemsSpawned.Add(item);


            ItemPickupInteractable pickup =  Instantiate(itemPickupInteractable, t.position, Quaternion.identity).GetComponent<ItemPickupInteractable>();
            pickup.item = item;
            startWeaponPickups.Add(pickup);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (ItemPickupInteractable weapon in startWeaponPickups)
        {
            if (weapon == null)
            {
                FindObjectOfType<RoomManager>().OpenDoors(true);
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
