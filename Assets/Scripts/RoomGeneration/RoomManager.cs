using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RoomManager : MonoBehaviour
{
    [Header("Rooms")]
    public Transform grid;
    public float roomGridSizeInUnits = 16;
    public float roomSpawningDelay = 1;
    [SerializeField] int maxRooms = 16;
    
    

    [Header("Room Spawning Prefabs")]
    [SerializeField] GameObject startRoom;
    [SerializeField] List<GameObject> rooms;
    List<GameObject> bottomOpeningrooms;
    List<GameObject> leftOpeningrooms;
    List<GameObject> topOpeningrooms;
    List<GameObject> rightOpeningrooms;

    [NonSerialized] public int amountOfRooms;

    int spawnedRooms = 1;
    GameObject[,] roomsArray;

    void Start()
    {
        SortRoomsByEntranceDir();

        //Spawn first room (it will spawn more rooms)
        Instantiate(startRoom);
    }

    private void SortRoomsByEntranceDir()
    {
        foreach (GameObject room in rooms)
        {
            List<Entrance> roomEntrances = room.GetComponent<Room>().entrances;
            foreach(Entrance entrance in roomEntrances)
            {
                GetListFromDirecton(entrance.direction).Add(room);
            }
        }
    }

    List<GameObject> _roomList;
    /// <summary>
    /// returns a random room based on the opening direction needed
    /// </summary>
    public GameObject GetRandomRoom(Direction direction)
    {
        _roomList = GetListFromDirecton(direction);
        return _roomList[UnityEngine.Random.Range(0, _roomList.Count)];
    }

    private bool RoomHasDirection(GameObject room, Direction direction)
    {
        
    }


    List<GameObject> GetListFromDirecton(Direction direction) 
    {
        switch (direction)
        {
            case (Direction.Bottom):   return bottomOpeningrooms;
            case (Direction.Left):     return leftOpeningrooms;
            case (Direction.Top):      return topOpeningrooms;
            case (Direction.Right):    return rightOpeningrooms;
        }
        return null;
    }

    public static Direction InvertDirection(Direction _originalDirection)
    {
        int enumLength = Enum.GetValues(typeof(Direction)).Length;
        int halfEnumLength = enumLength / 2;

        int originalValue = (int)_originalDirection;
        int invertedValue = (originalValue + halfEnumLength) % enumLength;

        return (Direction)invertedValue;
    }
}
