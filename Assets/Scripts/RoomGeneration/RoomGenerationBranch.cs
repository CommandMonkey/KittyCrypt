using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerationBranch : MonoBehaviour
{
    int _roomsToSpawn;
    public RoomGenerationBranch(int amountOfRooms)
    {
        this._roomsToSpawn = amountOfRooms;
    }

    private void Start()
    {
        
    }

    void SpawnRooms()
    {
        for (int i = 1; i < _roomsToSpawn; i++)
        {

        }

    }

    void SpawnRoomAtPosition()
    {

    }

    void moveInRandomDirection()
    {
        
    }

}
