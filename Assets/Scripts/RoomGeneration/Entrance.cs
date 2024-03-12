using System;
using UnityEngine;

public class Entrance : MonoBehaviour
{
    public Direction direction;
    [NonSerialized] public GameObject roomToSpawn;

    RoomManager roomManager;

    private void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
    }

    public void SpawnRoom()
    {
        if (roomToSpawn == null)
        {
            SpawnDoorCover();
            return;
        }

        roomManager.SpawnRoom(roomToSpawn, transform.position, RoomManager.InvertDirection(direction));
    }

    public void SpawnDoorCover()
    {
        roomManager.SpawnDoorCover(RoomManager.InvertDirection(direction), transform.position);
    }
}
