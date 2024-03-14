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
    [NonSerialized] public UnityEvent OnSpawnRoomWave;
    [NonSerialized] public List<Room> currentWaveRooms = new List<Room>();

    // private fields
    List<GameObject> _rooms = new List<GameObject>();
    bool _exitSpawned = false;


    void Start()
    {
        _rooms = levelData.GetRoomsList();

        //Spawn first room (it will spawn more rooms)
        Instantiate(levelData.startRoom, grid);

        InvokeRepeating("SpawnRoomWave", 1f, roomSpawningDelay);
    }


    void SpawnRoomWave() 
    {
        DistributeRooms();
        foreach(Room _room in currentWaveRooms)
        {
            foreach (Entrance _entrence in _room.entrances)
            {
                _entrence.SpawnRoom();
            }
        }
        currentWaveRooms = new List<Room>();
    }


    private void DistributeRooms()
    {
        foreach (Room _room in currentWaveRooms)
        {
            foreach (Entrance _entrance in _room.entrances)
            {
                _entrance.roomToSpawn = RetrieveRandomRoomPrefab(_entrance.direction);

            }
        }
    }


    /// <summary>
    /// returns a random room based on the opening direction needed
    /// </summary>
    public GameObject RetrieveRandomRoomPrefab(Direction direction)
    {
        if (_rooms.Count == 0)
        {
            return _exitSpawned ? levelData.endRoom : null;
        }

        int _rand = UnityEngine.Random.Range(0, _rooms.Count);
        GameObject result = _rooms[_rand];
        _rooms.Remove(result);
        return result;
    }


    public void SpawnRoom(GameObject _roomToSpawn, Vector3 _pos, Direction _roomEntranceDir)
    {

        Vector3 _entranceToZero = Vector3.zero - GetEntranceOfDir(_roomToSpawn.GetComponent<Room>(), _roomEntranceDir).transform.localPosition;

        Room newRoomInstance = Instantiate(_roomToSpawn, _pos + _entranceToZero, Quaternion.identity, grid)
            .GetComponent<Room>();

        // Remove entrance from the spawned room instance
        Entrance _roomMeetingEntrance = GetEntranceOfDir(newRoomInstance, _roomEntranceDir);
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



    public static Direction InvertDirection(Direction _originalDirection)
    {
        int enumLength = Enum.GetValues(typeof(Direction)).Length;
        int halfEnumLength = enumLength / 2;

        int originalValue = (int)_originalDirection;
        int invertedValue = (originalValue + halfEnumLength) % enumLength;

        return (Direction)invertedValue;
    }
}