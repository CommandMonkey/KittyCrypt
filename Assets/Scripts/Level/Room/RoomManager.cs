using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RoomManager : MonoBehaviour
{
    [Header("Rooms")]
    internal Transform grid;
    internal float roomSpawningDelay = 1;

    [Header("Room Spawning Prefabs")]
    internal LevelDataObject levelData;

    // public fields
    [NonSerialized] internal UnityEvent OnSpawnRoomWave;
    [NonSerialized] internal List<Room> currentWaveRooms = new List<Room>();
    [NonSerialized] internal List<Entrance> entrances = new List<Entrance>();

    // private fields
    List<GameObject> _rooms = new List<GameObject>();
    bool _exitSpawned = false;

    // Cached refs
    LevelManager levelManager;


    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();

        _rooms = levelData.GetRoomsList();

        if (levelManager.spawnRooms)
            StartRoomSpawning();
    }


    public void StartRoomSpawning()
    {
        //Spawn first room (it will spawn more rooms)
        Instantiate(levelData.startRoom, grid);

        StartCoroutine(SpawnRoomsRoutine());
    }

    IEnumerator SpawnRoomsRoutine()
    {
        int _previousRoomCount = 0;
        int _waveCount = 0;
        GameObject _roomToSpawn = null;
        while (_rooms.Count > 0 || currentWaveRooms.Count > 0)
        {
            Debug.Log("Distributing Rooms -----------------------------------");

            // If No rooms distributed last wave, terminate
            if (_waveCount > 5 && _rooms.Count == _previousRoomCount) 
            {
                Debug.LogWarning("TERMINATING ROOMSPAWNING, cant spawn all rooms, check room Pool");
                yield break;
            }
            _previousRoomCount = _rooms.Count;

            // Distribute Rooms
            foreach (Room _room in currentWaveRooms)
            {
                Debug.Log("Room: " + _room.name);
                foreach (Entrance _entrance in _room.entrances)
                {
                    _roomToSpawn = RetrieveRandomRoomPrefab(InvertDirection(_entrance.direction));

                    // Spawn Room
                    if (_roomToSpawn != null)
                        SpawnRoom(_roomToSpawn, _entrance.transform.position, InvertDirection(_entrance.direction));
                    else
                        _entrance.SpawnDoorCover();

                    //yield return new WaitForEndOfFrame();
                }
            }

            currentWaveRooms = new List<Room>();
            _waveCount++;
            yield return new WaitForSeconds(roomSpawningDelay);
        }

        Debug.Log("Room Spawning Done ---------------------------------");
    }


    // /////// ================= /////// //
    // ///////  HELPER FUNCTION  /////// //
    // /////// ================= /////// //
    int _randomTries = 0;
    public GameObject RetrieveRandomRoomPrefab(Direction _direction)
    {
        // Check null
        if (_rooms.Count == 0) return null;


        // Get random room
        int _rand = UnityEngine.Random.Range(0, _rooms.Count);
        GameObject result = _rooms[_rand];

        // Check if room has direction
        if (GetEntranceOfDir(result.GetComponent<Room>(), _direction))
        {
            _rooms.Remove(result);

            // Add exit room
            if (_rooms.Count == 0 && !_exitSpawned)
            {
                _rooms.Add(levelData.endRoom);
                _exitSpawned = true;
            }

            return result;
        } 
        else
        {
            // tries random 3 times, else it itterates over the lists to check if a valid room exists. else terminate
            _randomTries++;
            if (_randomTries > 3)
            {
                _randomTries = 0;
                foreach (GameObject _room in _rooms)
                {
                    if (GetEntranceOfDir(_room.GetComponent<Room>(), _direction)) 
                        return _room;
                }
                return null;
            }
            return RetrieveRandomRoomPrefab(_direction);
        }
    }

    public void AddRoomToList(GameObject _room)
    {
        _rooms.Add(_room);
    }


    public void SpawnRoom(GameObject _roomToSpawn, Vector3 _pos, Direction _roomEntranceDir)
    {

        Vector3 _entranceToZero = Vector3.zero - GetEntranceOfDir(_roomToSpawn.GetComponent<Room>(), _roomEntranceDir).transform.localPosition;

        Room newRoomInstance = Instantiate(_roomToSpawn, _pos + _entranceToZero, Quaternion.identity, grid)
            .GetComponent<Room>();

        // Give it its own prefab ref
        newRoomInstance.thisRoomPrefab = _roomToSpawn;

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
        if (room == null) return null;

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

    internal void CloseDoors()
    {
        foreach (Entrance entrance in entrances)
        {
            entrance.CloseDoor();
        }
    }
    internal void OpenDoors()
    {
        foreach (Entrance entrance in entrances)
        {
            entrance.OpenDoor();
        }
    }
}