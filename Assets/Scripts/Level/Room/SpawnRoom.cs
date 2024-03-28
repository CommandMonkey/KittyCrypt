using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRoom : Room
{
    [SerializeField] GameObject startWeaponPickup;
    

    // Update is called once per frame
    void Update()
    {
        if (startWeaponPickup == null)
        {
            FindObjectOfType<RoomManager>().OpenDoors();
            Destroy(this);
        }
    }
}
