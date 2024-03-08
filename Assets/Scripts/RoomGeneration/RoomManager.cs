using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomManager : MonoBehaviour
{
    [Header("Rooms")]
    public Transform grid;
    public float roomSpawningDelay = 1;

    [Header("Room Spawning Prefabs")]
    public LevelDataObject levelData;

    // public fields
    [NonSerialized] public UnityEvent OnSpawnRoomWave = new UnityEvent();
    [NonSerialized] public List<Room> currentWaveRooms;

    // private fields
    List<GameObject> bottomOpeningrooms = new List<GameObject>();
    List<GameObject> leftOpeningrooms = new List<GameObject>();
    List<GameObject> topOpeningrooms = new List<GameObject>();
    List<GameObject> rightOpeningrooms = new List<GameObject>();


    void Start()
    {
        SortRoomsByEntranceDir();

        //Spawn first room (it will spawn more rooms)
        Instantiate(levelData.startRoom, grid);

        InvokeRepeating("SpawnRoomWave", 1f, roomSpawningDelay);
    }

    void SpawnRoomWave() 
    {
        currentWaveRooms = new List<Room>();
        OnSpawnRoomWave.Invoke(); 
    }


    private void SortRoomsByEntranceDir()
    {
        foreach (GameObject room in levelData.rooms)
        {
            List<Entrance> roomEntrances = room.GetComponent<Room>().entrances;
            foreach (Entrance entrance in roomEntrances)
            {
                GetListFromDirecton(entrance.direction)?.Add(room);
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

        Room newRoomInstance = Instantiate(_room, entrance.transform.position + _entranceToZero, Quaternion.identity, grid)
            .GetComponent<Room>();

        // Remove entrance from the spawned room instance
        Entrance _roomMeetingEntrance = GetEntranceOfDir(newRoomInstance, _newRoomDir);
        if (_roomMeetingEntrance != null)
        {
            newRoomInstance.previousRoomEntrance = _roomMeetingEntrance;
            newRoomInstance.entrances.Remove(_roomMeetingEntrance);
        }
    }


    public void SpawnDoorCover(Direction _dir, Vector3 _pos)
    {
        GameObject roomBlocker = levelData.GetEntranceBlockerOfDir(_dir);
        Instantiate(roomBlocker, _pos, Quaternion.identity, grid);
    }


    Entrance GetEntranceOfDir(Room room, Direction dir)
    {
        foreach (Entrance entr in room.entrances)
        {
            if (entr.direction == dir) return entr;
        }
        return null;
    }


    List<GameObject> GetListFromDirecton(Direction direction)
    {
        switch (direction)
        {
            case (Direction.Bottom): return bottomOpeningrooms;
            case (Direction.Left): return leftOpeningrooms;
            case (Direction.Top): return topOpeningrooms;
            case (Direction.Right): return rightOpeningrooms;
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
