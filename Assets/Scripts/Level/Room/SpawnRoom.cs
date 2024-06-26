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
        if (gameSession.levelIndex != 0)
        {
            pickUpWeaponText.text = "Pick a Weapon, \nIf you want!";
        }

        foreach (Transform t in startWeaponPoints)
        {
            int tries = 0;

            GameObject item = null;
            while (item == null || itemsSpawned.Contains(item))
            {
                item = gameSession.levelSettings.GetRandomSpawnRoomItem();
                Debug.Log("SpawnRoom item: " + item.name);
                tries++;
                if (tries == 50) break;
            }
            itemsSpawned.Add(item);


            ItemPickupInteractable pickup =  Instantiate(itemPickupInteractable, t).GetComponent<ItemPickupInteractable>();
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
                pickUpWeaponText.text = "Find the Rat King\nDon't forget you can Dash";
                Destroy(pickUpWeaponText.gameObject, 3f);
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
