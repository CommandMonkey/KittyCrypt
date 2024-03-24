using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    [NonSerialized] protected UnityEvent OnPlayerEnter;

    

    // Cached references
    protected RoomManager roomManager;
    BoxCollider2D boxCollider;

    private void Awake()
    {
        OnPlayerEnter = new UnityEvent();
    }

    void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (IsOverlapping())
        {
            previousRoomEntrance.hasConnectedRoom = false;
            Die();
            return;
        }

        newlySpawned = false;
        roomManager.currentWaveRooms.Add(this);
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



    private void Die()
    {
        roomManager.AddRoomToList(thisRoomPrefab);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            OnPlayerEnter.Invoke();
    }

}
