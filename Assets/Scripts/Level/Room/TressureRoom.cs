using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TressureRoom : Room
{
    [SerializeField] List<Transform> itemSpawns;

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
        GameObject item = gameSession.levelSettings.GetRandomItem();
        Instantiate(item, t);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
