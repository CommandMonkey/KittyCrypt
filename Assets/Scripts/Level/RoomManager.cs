using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

// -- Basic information RoomManager -- //
// Tries to spawn rooms in a maze like structure by trial and error. It tries addign room after room until it works
// 
// A room is a prefab with:
//  - a box collider that show the bounds of a room. 
//  - child objects positioned at the door locations with an Entrance component attached
//
// Room GO example structure:
// CourtRoom
// -- BoundsCollider
// -- EntranceWest
// -- EntranceNorth
// -- EntranceEast



public class RoomManager : MonoBehaviour
{
#region Fields & Properties
    
    [Header("General")]
    [SerializeField] Transform grid;
    [SerializeField] AudioClip closeDoorAudio;
    [SerializeField] AudioClip openDoorAudio;

    // public fields
    [NonSerialized] internal RoomGenObject roomGenSettings;
    [NonSerialized] internal UnityEvent onRoomSpawningDone;
    [NonSerialized] internal UnityEvent onEntranceExit;
    [NonSerialized] internal List<Entrance> entrances = new List<Entrance>();

    // private fields
    List<GameObject> roomsToSpawn = new List<GameObject>();
    List<Room> spawnedRooms = new List<Room>();
    Dictionary<Entrance, List<string>> entranceRoomFailNames = new Dictionary<Entrance, List<string>>();
    ContactFilter2D roomsFilter;
    int totalTries = 0;

    // Cached refs
    GameSession gameSession;
    AudioSource audioScource;
    Player player;
    
#endregion

#region Initialization
    private void Awake()
    {
        onRoomSpawningDone = new UnityEvent();
        onEntranceExit = new UnityEvent();
    }

    void Start()
    {
        gameSession = GameSession.Instance;
        audioScource = GetComponent<AudioSource>();
        player = gameSession.Player;
        
        // Setup Generation Data
        roomGenSettings = gameSession.levelSettings.roomGenSettings;
        roomsFilter = CreateRoomsFilter();

        StartRoomSpawning();
    }
    
    
    // Is called on start nd recursively if the room spawning fails. Terminates after 100 fails to prevent crash (kinda bad)
    private void StartRoomSpawning()
    {
        GameSession.state = GameSession.GameState.Loading; // Bad reference

        roomsToSpawn = roomGenSettings.GetRoomsList();
        Debug.Log("count: " +  roomsToSpawn.Count);
        spawnedRooms.Clear();

        totalTries++;

        if (totalTries > 100 || roomsToSpawn == new List<GameObject>())
        {
            Debug.LogWarning("ROOM SPAWNING SUCKS AND FAILS");
            return;
        }
        //Spawn first room (it will spawn more rooms)
        spawnedRooms.Add(Instantiate(roomGenSettings.startRoom, grid).GetComponent<Room>());
        StartCoroutine(SpawnRoomsRoutine());
        
    }
    
#endregion

#region Room Spawning

    IEnumerator SpawnRoomsRoutine()
    {
        int previousRoomCount = 0;
        int waveCount = 0;
        
        // While there is rooms to spawn
        while (roomsToSpawn.Count > 0)
        {
            // Debug
            waveCount++;
            Debug.Log("RoomSpawning Wave: " + waveCount);

            if (ShouldTerminateRoomSpawning(waveCount, previousRoomCount))
            {
                TerminateAndRetryRoomSpawning(); // Break point
                yield break;
            }
            previousRoomCount = roomsToSpawn.Count;

            // Shuffle
            List<GameObject> shuffledRooms = ShuffleRoomsToSpawn();
            yield return new WaitForEndOfFrame();

            // Try Spawn rooms this wave
            foreach (Entrance entrance in GetShuffledUnconnectedEntrances())
            {
                if (TrySpawnRoomsAtEntrance(shuffledRooms, entrance, roomsFilter))
                    break;
            }
        }
        
        // Finnishing
        yield return null;
        yield return null;
        SpawnEndRooms(); 
        yield return new WaitForEndOfFrame();

        // Cleanup
        SpawnDoorCoversForUnconnectedEntrances();
        if (gameSession.levelIndex == 0) CloseDoors(true);
        onRoomSpawningDone.Invoke();
        player.transform.position = Vector3.zero; // BAD, VERY BAD
    }

    bool ShouldTerminateRoomSpawning(int waveCount, int previousRoomCount)
    {
        // there has been 10 waves and it could not place any rooms last wave
        return waveCount > 10 && roomsToSpawn.Count == previousRoomCount;
    }

    void TerminateAndRetryRoomSpawning()
    {
        Debug.Log("Terminating roomSpawning");
        DestroyAllRooms();
        entrances.Clear();
        StartRoomSpawning();
    }

    bool TrySpawnRoomsAtEntrance(List<GameObject> shuffledRooms, Entrance entrance, ContactFilter2D roomsFilter)
    {
        Direction invDirection = InvertDirection(entrance.direction);
        Vector3 entrancePosition = entrance.transform.position;

        foreach (GameObject roomObj in shuffledRooms)
        {
            Room roomComponent = roomObj.GetComponent<Room>();
            Entrance roomEntrance = GetEntranceOfDir(roomComponent, invDirection);

            if (roomEntrance != null &&
               (!entranceRoomFailNames.ContainsKey(entrance) || !entranceRoomFailNames[entrance].Contains(roomObj.name)))
            {
                Vector3 entranceToZero = Vector3.zero - roomEntrance.transform.localPosition;
                Vector3 spawnPosition = entrancePosition + entranceToZero;

                bool isTouchingRoom = GameHelper.IsBoxColliderTouching(spawnPosition, roomObj.GetComponent<BoxCollider2D>(), roomsFilter);

                if (!isTouchingRoom)
                {
                    SpawnRoom(roomObj, spawnPosition, entrance);
                    shuffledRooms.Remove(roomObj);
                    roomsToSpawn.Remove(roomObj);
                    return true;
                }
                else
                {
                    HandleFailedRoomSpawn(entrance, roomObj);
                }
            }
        }
        return false;
    }

    void HandleFailedRoomSpawn(Entrance entrance, GameObject roomObj)
    {
        // Save Room names that are incompatable with an entrance to reduce number of collider checks
        if (entranceRoomFailNames.ContainsKey(entrance))
        {
            entranceRoomFailNames[entrance].Add(roomObj.name);
        }
        else
        {
            entranceRoomFailNames.Add(entrance, new List<string>() { roomObj.name });
        }
    }

    void SpawnEndRooms()
    {
        List<GameObject> endRooms = new List<GameObject>(roomGenSettings.endRooms);
        List<Vector2> avoidPositions = new List<Vector2>() { Vector2.zero };

        foreach (GameObject _room in endRooms)
        {
            bool hasSpawned = false;
            List<Entrance> furthestAway = GetUnconnectedEntrancesSortedByDistance(avoidPositions);
            if (furthestAway == null || furthestAway.Count < 0)
            {
                TerminateRoomSpawning();
                break;
            }

            furthestAway.Reverse();

            foreach (Entrance entr in furthestAway)
            {
                Direction invDirection = InvertDirection(entr.direction);
                Room roomComponent = _room.GetComponent<Room>();
                Entrance roomEntrance = GetEntranceOfDir(roomComponent, invDirection);
                if (roomEntrance != null)
                {
                    Vector3 entranceToZero = Vector3.zero - roomEntrance.transform.localPosition;
                    Vector3 spawnPosition = entr.transform.position + entranceToZero;

                    bool isTouchingRoom = GameHelper.IsBoxColliderTouching(spawnPosition, _room.GetComponent<BoxCollider2D>(), roomsFilter);
                    if (!isTouchingRoom)
                    {
                        SpawnRoom(_room, spawnPosition, entr);
                        avoidPositions.Add(spawnPosition);
                        hasSpawned = true;
                        break;
                    }
                }
            }
            if (!hasSpawned)
            {
                TerminateRoomSpawning();
                break;
            }
        }
    }

    void SpawnDoorCoversForUnconnectedEntrances()
    {
        foreach (Entrance entrance in entrances)
        {
            if (!entrance.hasConnectedRoom)
                entrance.SpawnDoorCover();
        }
    }

#endregion


    // /////// ================= /////// //
    // ///////  HELPER FUNCTION  /////// //
    // /////// ================= /////// //

#region Helper Methods

    List<GameObject> ShuffleRoomsToSpawn()
    {
        return GameHelper.ShuffleList(roomsToSpawn);
    }

    IEnumerable<Entrance> GetShuffledUnconnectedEntrances()
    {
        return GameHelper.ShuffleList(entrances);
    }

    public List<Entrance> GetUnconnectedEntrancesSortedByDistance(List<Vector2> avoidPositions)
    {
        Dictionary<Entrance, float> entranceDistances = new Dictionary<Entrance, float>();

        foreach (Entrance entr in entrances)
        {
            if (!entr.hasConnectedRoom)
            {
                float totalDistance = 0f;
                foreach (Vector2 avoidPos in avoidPositions)
                {
                    float distance = Vector2.Distance(entr.transform.position, avoidPos);
                    totalDistance += distance;
                }
                entranceDistances.Add(entr, totalDistance);
            }
        }

        // Sort the anslutna entr�er baserat p� avst�ndet fr�n avoidPositions
        List<Entrance> sortedEntrances = new List<Entrance>(entranceDistances.Keys);
        sortedEntrances.Sort((entr1, entr2) =>
            entranceDistances[entr1].CompareTo(entranceDistances[entr2]));

        return sortedEntrances;
    }

    ContactFilter2D CreateRoomsFilter()
    {
        return new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Room"),
            useTriggers = true
        };
    }

    void DestroyAllRooms()
    {
        foreach (Room room in spawnedRooms)
        {
            //sforeach(Entrance entr in room.entrances) entr.Die();
            Destroy(room.gameObject);
        }
    }

    public void AddRoomToList(GameObject _room)
    {
        roomsToSpawn.Add(_room);
    }


    public void SpawnRoom(GameObject _roomToSpawn, Vector3 _position, Entrance _entrance)
    {

        Room _newRoomInstance = Instantiate(_roomToSpawn, _position, Quaternion.identity, grid)
            .GetComponent<Room>();
        spawnedRooms.Add(_newRoomInstance);


        // Remove entrance from the spawned room instance
        Entrance _roomMeetingEntrance = GetEntranceOfDir(_newRoomInstance, InvertDirection(_entrance.direction));
        if (_roomMeetingEntrance != null)
        {
            _newRoomInstance.previousRoomEntrance = _entrance;
            _newRoomInstance.entrances.Remove(_roomMeetingEntrance);
            _roomMeetingEntrance.Die();
        }
        _entrance.OnConnectedRoomSpawned();
    }


    public void SpawnDoorCover(Direction _dir, Vector3 _pos)
    {
        GameObject _roomBlocker = roomGenSettings.GetEntranceBlockerOfDir(_dir);
        GameObject _instance = Instantiate(_roomBlocker, _pos, Quaternion.identity, grid);
        foreach (Tilemap _tilemap in GameHelper.GetComponentsInAllChildren<Tilemap>(_instance.transform))
        {
            _tilemap.color = gameSession.levelSettings.wallColor;
        }
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


    public void CloseDoors(bool silent = false)
    {
        foreach (Entrance entrance in entrances)
        {
            entrance.CloseDoor();
        }
        if (!silent) audioScource.PlayOneShot(closeDoorAudio);
    }


    public void OpenDoors(bool silent = false)
    {
        foreach (Entrance entrance in entrances)
        {
            entrance.OpenDoor();
        }
        if (!silent) audioScource.PlayOneShot(openDoorAudio, 0.4f);
    }
#endregion
}