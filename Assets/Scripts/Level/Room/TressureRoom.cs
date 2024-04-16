using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TressureRoom : Room
{
    [SerializeField] List<Transform> itemSpawns;
    [SerializeField] GameObject pickupPrefab;

    List<ItemPickupInteractable> interactables = new List<ItemPickupInteractable>();

    // Start is called before the first frame update
    protected override void RoomStart()
    {
        
        foreach (Transform t in itemSpawns) 
        {
            SpawnItem(t);
        }
    }

    private void SpawnItem(Transform t)
    {
        ItemPickupInteractable pickup = Instantiate(pickupPrefab, t).GetComponent<ItemPickupInteractable>();
        interactables.Add(pickup);
    }

    // Update is called once per frame
    void Update()
    {
        foreach(ItemPickupInteractable interact in interactables)
        {
            if (interact == null)
            {
                PopAllInteractabes();
                break;
            }
        }
    }

    private void PopAllInteractabes()
    {
        foreach (ItemPickupInteractable interact in interactables)
        {
            if (interact !=  null)
            {
                interact.PopAndDie();
            }
        }
    }
}
