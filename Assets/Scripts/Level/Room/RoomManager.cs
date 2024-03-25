using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RoomManager : MonoBehaviour
{
    [Header("Rooms")]
    [SerializeField] internal Transform grid;
    [SerializeField] internal float roomSpawningDelay = 1;

    [Header("Room Spawning Prefabs")]
    [SerializeField] internal LevelDataObject levelData;

    // public fields
    [NonSerialized] internal UnityEvent OnSpawnRoomDone;
    [NonSerialized] internal List<Entrance> entrances = new List<Entrance>();

    // private fields
    List<GameObject> _rooms = new List<GameObject>();
    bool _exitSpawned = false;

    // Cached refs
    LevelManager levelManager;

    void Start()
    {
        OnSpawnRoomDone = new UnityEvent();
        levelManager = FindObjectOfType<LevelManager>();

        _rooms = levelData.GetRoomsList();

        if (levelManager.spawnRooms)
            StartRoomSpawning();
    }


    public void StartRoomSpawning()
    {
        levelManager.state = LevelManager.LevelState.Loading;

        //Spawn first room (it will spawn more rooms)
        Instantiate(levelData.startRoom, grid);

        StartCoroutine(SpawnRoomsRoutine());
    }

    IEnumerator SpawnRoomsRoutine()
    {
        // Define the contact filter for room colliders
        ContactFilter2D roomsFilter = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Room"),
            useTriggers = true
        };

        int _previousRoomCount = 0;
        int _waveCount = 0;

        while (_rooms.Count > 0)
        {
            Debug.Log("wave: " + _waveCount);

            // Check termination condition
            if (_waveCount > 5 && _rooms.Count == _previousRoomCount)
            {
                Debug.LogWarning("TERMINATING ROOMSPAWNING, cant spawn all rooms, check room Pool");
                break;
            }
            _previousRoomCount = _rooms.Count;

            // Shuffle the list of rooms to spawn
            List<GameObject> _roomsToSpawn = GameHelper.ShuffleList(_rooms);
            yield return new WaitForEndOfFrame();

            // Iterate over entrances
            foreach (Entrance _entrance in GameHelper.ShuffleList(entrances))
            {
                Direction _invDirection = InvertDirection(_entrance.direction);
                Vector3 _entrPos = _entrance.gameObject.transform.position;

                // Check if the entrance has a connected room
                if (!_entrance.hasConnectedRoom)
                {
                    foreach (GameObject _room in _roomsToSpawn)
                    {
                        Room _roomComponent = _room.GetComponent<Room>();
                        Entrance _roomEntrance = GetEntranceOfDir(_roomComponent, _invDirection);

                        // Check if the room can be spawned at this entrance
                        if (_roomEntrance != null)
                        {
                            Vector3 _entranceToZero = Vector3.zero - _roomEntrance.transform.localPosition;
                            Vector3 _spawnPosition = _entrPos + _entranceToZero;

                            // Check if the room collider would touch anything at the spawn position
                            bool _isTouchingRoom = GameHelper.IsBoxColliderTouching(_spawnPosition, _room.GetComponent<BoxCollider2D>(), roomsFilter);

                            // If the room collider doesn't touch anything, spawn the room
                            if (!_isTouchingRoom)
                            {
                                SpawnRoom(_room, _spawnPosition, _entrance);
                                _roomsToSpawn.Remove(_room);
                                _rooms.Remove(_room);
                                break;
                            }
                        }
                    }
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForEndOfFrame();
            }
        }

            yield return new WaitForEndOfFrame();

        foreach (Entrance entr in entrances)
        {
            if(!entr.hasConnectedRoom) entr.SpawnDoorCover();
        }

        Debug.Log("Room Spawning Done ---------------------------------");
        OnSpawnRoomDone.Invoke();
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


    public void SpawnRoom(GameObject _roomToSpawn, Vector3 _position, Entrance _entrance)
    {

        Room newRoomInstance = Instantiate(_roomToSpawn, _position, Quaternion.identity, grid)
            .GetComponent<Room>();


        // Remove entrance from the spawned room instance
        Entrance _roomMeetingEntrance = GetEntranceOfDir(newRoomInstance, InvertDirection(_entrance.direction));
        if (_roomMeetingEntrance != null)
        {
            newRoomInstance.previousRoomEntrance = _entrance;
            newRoomInstance.entrances.Remove(_roomMeetingEntrance);
            _roomMeetingEntrance.Die();
        }
        _entrance.hasConnectedRoom = true;
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

    public void CloseDoors()
    {
        foreach (Entrance entrance in entrances)
        {
            entrance.CloseDoor();
        }
    }
    public void OpenDoors()
    {
        foreach (Entrance entrance in entrances)
        {
            Debug.Log("manager closing door");
            entrance.OpenDoor();
        }
    }
}