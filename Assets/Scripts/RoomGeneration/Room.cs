using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Direction
{
    Bottom,
    Left,
    Top, 
    Right
}

public enum RoomType
{
    Start, 
    End,
    Normal
}

public class Room : MonoBehaviour
{
    public List<Entrance> entrances;
    public bool newlySpawned = true;
    public Entrance previousRoomEntrance;

    // Cached references
    RoomManager roomManager;
    BoxCollider2D boxCollider;
    

    void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (IsOverlapping())
        {
            Debug.Log("ROOM ION THA W");
            roomManager.SpawnDoorCover(RoomManager.InvertDirection(previousRoomEntrance.direction), previousRoomEntrance.transform.position);
            Die();
            return;
        }

        newlySpawned = false;
        roomManager.currentWaveRooms.Add(this);
        roomManager.OnSpawnRoomWave.AddListener(SpawnConnectingRooms);
    }

    private bool IsOverlapping()
    {
        Collider2D[] results = new Collider2D[10]; 
        int numColliders = boxCollider.OverlapCollider(new ContactFilter2D(), results);
        Debug.Log(numColliders);

        // Check if any colliders are detected
        if (numColliders > 0)
        {
            foreach (Collider2D collider in results)
            {
                // WAS DOING, !room.newlySpawned throws a Null ref error. IDK WHYYYYYYYYYYY!!

                if (collider?.gameObject.CompareTag("Room") ?? false) 
                {
                    if (!collider.gameObject.GetComponent<Room>().newlySpawned)
                        return true;
                }
            }
        }
        return false;
    }

    void SpawnConnectingRooms()
    {
        foreach(Entrance entrence in entrances)
        {
            if (entrence.connection == null)
                roomManager.SpawnRoomConectedToEntrance(entrence);
        }

        roomManager.OnSpawnRoomWave.RemoveListener(SpawnConnectingRooms);
    }

    private void Die()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
