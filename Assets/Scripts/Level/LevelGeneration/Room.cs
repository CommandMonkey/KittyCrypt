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
    [NonSerialized] public bool newlySpawned = true;
    [NonSerialized] public Entrance previousRoomEntrance;
    [NonSerialized] public GameObject thisRoomPrefab;

    // Cached references
    RoomManager roomManager;
    BoxCollider2D boxCollider;
    

    void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (IsOverlapping())
        {
            roomManager.SpawnDoorCover(RoomManager.InvertDirection(previousRoomEntrance.direction), previousRoomEntrance.transform.position);
            previousRoomEntrance.Die();
            Die();
            return;
        }

        newlySpawned = false;
        roomManager.currentWaveRooms.Add(this);
    }

    public bool IsOverlapping()
    {
        Collider2D[] results = new Collider2D[10]; 
        int numColliders = boxCollider.OverlapCollider(new ContactFilter2D(), results);

        // Check if any colliders are detected
        if (numColliders > 0)
        {
            foreach (Collider2D collider in results)
            {
                // Check if Room
                if (collider?.gameObject.CompareTag("Room") ?? false) 
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Die()
    {
        roomManager.AddRoomToList(thisRoomPrefab);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    void CloseDoors()
    {
        foreach(Entrance entr in entrances)
        {

        }
        previousRoomEntrance.CloseDoor();
    }
}
