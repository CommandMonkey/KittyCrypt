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
    public List<DirectionGameObjectPair> doorCovers;


    List<GameObject> bottomOpeningrooms = new List<GameObject>(1);
    List<GameObject> leftOpeningrooms = new List<GameObject>(1);
    List<GameObject> topOpeningrooms = new List<GameObject>(1);
    List<GameObject> rightOpeningrooms = new List<GameObject>(1);

    [NonSerialized] public int amountOfRooms;

    int spawnedRooms = 1;
    GameObject[,] roomsArray;

    void Start()
    {
        SortRoomsByEntranceDir();

        //Spawn first room (it will spawn more rooms)
        Instantiate(startRoom, grid);
    }

    private void SortRoomsByEntranceDir()
    {
        Debug.Log("----------");
        foreach (GameObject room in rooms)
        {
            Debug.Log("Room: " + room.name);
            Debug.Log("Entrance count: " + room.GetComponent<Room>().entrances.Count);
            List<Entrance> roomEntrances = room.GetComponent<Room>().entrances;
            foreach(Entrance entrance in roomEntrances)
            {
                Debug.Log("Direction: "+entrance.direction);
                GetListFromDirecton(entrance.direction)?.Add(room);
            }
        }
        Debug.Log("----------");
    }

    List<GameObject> _roomList;
    /// <summary>
    /// returns a random room based on the opening direction needed
    /// </summary>
    public GameObject GetRandomRoom(Direction direction)
    {
        _roomList = GetListFromDirecton(direction);
        if (_roomList.Count == 0)
        {
            return null;
        }
        

        int _rand = UnityEngine.Random.Range(0, _roomList.Count);
        return _roomList[_rand];
    }

    GameObject _room;
    public void SpawnRoomConectedToEntrance(Entrance entrance)
    {
        Direction _newRoomDir = InvertDirection(entrance.direction);
        _room = GetRandomRoom(_newRoomDir);
        if (_room == null) // null check
        {
            return;
        }

        Vector3 _entranceToZero = Vector3.zero - GetEntranceOfDir(_room.GetComponent<Room>(), _newRoomDir).transform.localPosition;

        GameObject newRoomInstance = Instantiate(_room, entrance.transform.position + _entranceToZero, Quaternion.identity, grid);

        // Remove entrance from the spawned room instance
        Entrance _roomMeetingEntrance = GetEntranceOfDir(newRoomInstance.GetComponent<Room>(), _newRoomDir);
        if (_roomMeetingEntrance != null)
        {
            newRoomInstance.GetComponent<Room>().entrances.Remove(_roomMeetingEntrance);
        }
    }

    public void SpawnDoorCover(Direction _dir, Vector3 pos)
    {

    }

    Entrance GetEntranceOfDir(Room room, Direction dir)
    {
        foreach(Entrance entr in room.entrances)
        {
            if (entr.direction == dir) return entr;
        }
        return null;
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


[System.Serializable]
public class DirectionGameObjectPair
{
    public Direction direction;
    public GameObject gameObject;
}
