using System;
using UnityEngine;

public class Entrance : MonoBehaviour
{
    public Direction direction;
    [NonSerialized] public GameObject roomToSpawn;

    bool doorOpen = true;

    RoomManager roomManager;

    private void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
        roomManager.entrances.Add(this);
    }


    public void SpawnDoorCover()
    {
        roomManager.SpawnDoorCover(direction, transform.position);
        Die();
    }

    public void Die()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    internal void CloseDoor()
    {
        
    }

    private void UpdateDoorState()
    {
        
    }
}
