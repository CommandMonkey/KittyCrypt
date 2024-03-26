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

        int previousRoomCount = 0;
        int waveCount = 0;

        while (_rooms.Count > 0)
        {
            Debug.Log("Wave: " + waveCount);

            // Check termination condition
            if (waveCount > 5 && _rooms.Count == previousRoomCount)
            {
                Debug.LogWarning("TERMINATING ROOMSPAWNING. Can't spawn all rooms. Check room Pool.");
                break;
            }
            previousRoomCount = _rooms.Count;

            // Shuffle the list of rooms to spawn
            List<GameObject> roomsToSpawn = GameHelper.ShuffleList(_rooms);
            yield return new WaitForEndOfFrame();

            // Iterate over entrances
            foreach (Entrance entrance in GameHelper.ShuffleList(entrances))
            {
                Direction invDirection = InvertDirection(entrance.direction);
                Vector3 entrancePosition = entrance.gameObject.transform.position;

                // Check if the entrance has a connected room
                if (!entrance.hasConnectedRoom)
                {
                    foreach (GameObject roomObj in roomsToSpawn)
                    {
                        Room roomComponent = roomObj.GetComponent<Room>();
                        Entrance roomEntrance = GetEntranceOfDir(roomComponent, invDirection);

                        // Check if the room can be spawned at this entrance
                        if (roomEntrance != null)
                        {
                            Vector3 entranceToZero = Vector3.zero - roomEntrance.transform.localPosition;
                            Vector3 spawnPosition = entrancePosition + entranceToZero;

                            // Check if the room collider would touch anything at the spawn position
                            bool isTouchingRoom = GameHelper.IsBoxColliderTouching(spawnPosition, roomObj.GetComponent<BoxCollider2D>(), roomsFilter);

                            // If the room collider doesn't touch anything, spawn the room
                            if (!isTouchingRoom)
                            {
                                SpawnRoom(roomObj, spawnPosition, entrance);
                                roomsToSpawn.Remove(roomObj);
                                _rooms.Remove(roomObj);
                                break;
                            }
                        }
                    }
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForEndOfFrame();
            }
        }

        // Spawn door covers for entrances without connected rooms
        foreach (Entrance entrance in entrances)
        {
            if (!entrance.hasConnectedRoom)
                entrance.SpawnDoorCover();
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