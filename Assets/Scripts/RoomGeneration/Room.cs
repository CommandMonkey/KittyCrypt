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

    // Cached references
    RoomManager roomManager;
    Collider2D compositeCollider;

    void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
        compositeCollider = GetComponent<Collider2D>();

        if (IsOverlapping())
        {
            Die();
        }

        Invoke("SpawnConnectingRooms", 3);
    }

    private bool IsOverlapping()
    {

        Collider2D[] results = new Collider2D[10]; 
        int numColliders = compositeCollider.OverlapCollider(new ContactFilter2D(), results);

        // Check if any colliders are detected
        if (numColliders > 0)
        {
            foreach (Collider2D collider in results)
            {
                if (collider != null && collider.gameObject.CompareTag("Room"))
                {
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
    }

    private void Die()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
