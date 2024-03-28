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

public class Room : MonoBehaviour
{
    public List<Entrance> entrances;

    [NonSerialized] public Entrance previousRoomEntrance;
    [NonSerialized] public GameObject thisRoomPrefab;
    [NonSerialized] protected UnityEvent OnPlayerEnter;

    // Cached references
    RoomManager roomManager;
    BoxCollider2D boxCollider;

    private void Awake()
    {
        OnPlayerEnter = new UnityEvent();

    }

    void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
        boxCollider = GetComponent<BoxCollider2D>();

        entrances = new List<Entrance>(FindObjectsByType<Entrance>(FindObjectsSortMode.None));

        //Invoke("CheckSpawningConditions", .01f);
    }


    //private bool IsOverlapping()
    //{
    //    ContactFilter2D filter = new ContactFilter2D();
    //    filter.layerMask = LayerMask.GetMask("Room");
    //    filter.useTriggers = true;

    //    Collider2D[] results = new Collider2D[10];
    //    int numColliders = boxCollider.OverlapCollider(filter, results);

    //    // Check if any colliders are detected
    //    if (numColliders > 0)
    //    {
    //        for (int i = 0; i < numColliders; i++)
    //        {
    //            Collider2D collider = results[i];
    //            if (collider != null)
    //            {
    //                if (collider.gameObject.CompareTag("Room"))
    //                {
    //                    Debug.Log("COLLIDING with Room: " + collider.gameObject.name);
    //                    return true;
    //                }
    //            }
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("No overlapping colliders found.");
    //    }

    //    return false;
    //}


    private void Die()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collide!!!: " + name + " with: " + collision.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            OnPlayerEnter.Invoke();
            Debug.Log("player enter room:" + gameObject.name);  
        }
            
    }

    bool IsPlayerInside()
    {
        throw new NotImplementedException();
    }

}
