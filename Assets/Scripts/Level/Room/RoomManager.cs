using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RoomManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField] Transform grid;
    [SerializeField] AudioClip closeDoorAudio;
    [SerializeField] AudioClip openDoorAudio;

    [Header("Room Spawning Prefabs")]
    [SerializeField] internal LevelDataObject levelData;

    // public fields
    [NonSerialized] internal UnityEvent OnRoomSpawningDone;
    [NonSerialized] internal List<Entrance> unconnectedEntrances = new List<Entrance>();

    // private fields
    List<GameObject> roomsToSpawn = new List<GameObject>();
    List<Room> spawnedRooms = new List<Room>();
    Dictionary<Entrance, List<string>> entranceRoomFailNames = new Dictionary<Entrance, List<string>>();

    // Cached refs
    LevelManager levelManager;
    AudioSource audioScource;

    private void Awake()
    {
        OnRoomSpawningDone = new UnityEvent();
    }

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        audioScource = levelManager.GetComponent<AudioSource>();



        if (levelManager.spawnRooms)
            StartRoomSpawning();
    }

    public void StartRoomSpawning()
    {
        levelManager.state = LevelManager.LevelState.Loading;

        roomsToSpawn = levelData.GetRoomsList();
        spawnedRooms.Clear();

        //Spawn first room (it will spawn more rooms)
        spawnedRooms.Add(Instantiate(levelData.startRoom, grid).GetComponent<Room>());

        StartCoroutine(SpawnRoomsRoutine());
    }

    IEnumerator SpawnRoomsRoutine()
    {
        ContactFilter2D roomsFilter = CreateRoomsFilter();
        int previousRoomCount = 0;
        int waveCount = 0;

        while (roomsToSpawn.Count > 0)
        {
            waveCount++;
            Debug.Log("Wave: " + waveCount);

            if (ShouldTerminateRoomSpawning(waveCount, previousRoomCount))
            {
                TerminateRoomSpawning();
                yield break;
            }

            List<GameObject> shuffledRooms = ShuffleRoomsToSpawn();
            yield return new WaitForEndOfFrame();

            foreach (Entrance entrance in GetShuffledUnconnectedEntrances())
            {
                if (TrySpawnRoomAtEntrance(shuffledRooms, entrance, roomsFilter))
                    break;
            }
        }

        SpawnEndRoom();
        yield return new WaitForEndOfFrame();

        SpawnDoorCoversForUnconnectedEntrances();
        levelManager.state = LevelManager.LevelState.Running;
        CloseDoors(true);
        OnRoomSpawningDone.Invoke();
    }

    bool ShouldTerminateRoomSpawning(int waveCount, int previousRoomCount)
    {
        return waveCount > 10 && roomsToSpawn.Count == previousRoomCount;
    }

    void TerminateRoomSpawning()
    {
        LogWarningPerRooms("TERMINATING ROOM SPAWNING, can't spawn room: ");
        DestroyAllRooms();
        StartRoomSpawning();
    }

    bool TrySpawnRoomAtEntrance(List<GameObject> shuffledRooms, Entrance entrance, ContactFilter2D roomsFilter)
    {
        Direction invDirection = InvertDirection(entrance.direction);
        Vector3 entrancePosition = entrance.gameObject.transform.position;

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
        if (entranceRoomFailNames.ContainsKey(entrance))
        {
            entranceRoomFailNames[entrance].Add(roomObj.name);
        }
        else
        {
            entranceRoomFailNames.Add(entrance, new List<string>() { roomObj.name });
        }
    }

    void SpawnEndRoom()
    {
        Entrance furthestAway = GetFurthestUnconnectedEntrance();
        if (furthestAway != null)
        {
            Room endRoomComponent = levelData.endRoom.GetComponent<Room>();
            Entrance endRoomEntrance = GetEntranceOfDir(endRoomComponent, InvertDirection(furthestAway.direction));
            Vector3 endSpawnPosition = furthestAway.transform.position + (Vector3.zero - endRoomEntrance.transform.localPosition);
            SpawnRoom(levelData.endRoom, endSpawnPosition, furthestAway);
        }
    }

    void SpawnDoorCoversForUnconnectedEntrances()
    {
        foreach (Entrance entrance in unconnectedEntrances)
        {
            if (!entrance.hasConnectedRoom)
                entrance.SpawnDoorCover();
        }
    }


    // /////// ================= /////// //
    // ///////  HELPER FUNCTION  /////// //
    // /////// ================= /////// //


    List<GameObject> ShuffleRoomsToSpawn()
    {
        return GameHelper.ShuffleList(roomsToSpawn);
    }

    IEnumerable<Entrance> GetShuffledUnconnectedEntrances()
    {
        return GameHelper.ShuffleList(unconnectedEntrances);
    }

    Entrance GetFurthestUnconnectedEntrance()
    {
        Entrance furthestAway = null;
        float furthestAwayDistance = 0;

        foreach (Entrance entr in unconnectedEntrances)
        {
            if (!entr.hasConnectedRoom)
            {
                float distance = (Vector3.zero - entr.transform.position).magnitude;
                if (distance > furthestAwayDistance)
                {
                    furthestAway = entr;
                    furthestAwayDistance = distance;
                }
            }
        }

        return furthestAway;
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
            foreach(Entrance entr in room.entrances) entr.Die();
            Destroy(room.gameObject);
        }
    }

    void LogWarningPerRooms(string pretext = "")
    {
        foreach(GameObject room in roomsToSpawn)
        {
            Debug.Log(pretext + room.name);
        }
    }

    public void AddRoomToList(GameObject _room)
    {
        roomsToSpawn.Add(_room);
    }


    public void SpawnRoom(GameObject _roomToSpawn, Vector3 _position, Entrance _entrance)
    {

        Room newRoomInstance = Instantiate(_roomToSpawn, _position, Quaternion.identity, grid)
            .GetComponent<Room>();
        spawnedRooms.Add(newRoomInstance);


        // Remove entrance from the spawned room instance
        Entrance _roomMeetingEntrance = GetEntranceOfDir(newRoomInstance, InvertDirection(_entrance.direction));
        if (_roomMeetingEntrance != null)
        {
            newRoomInstance.previousRoomEntrance = _entrance;
            newRoomInstance.entrances.Remove(_roomMeetingEntrance);
            _roomMeetingEntrance.Die();
        }
        _entrance.OnConnectedRoomSpawned();
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


    public void CloseDoors(bool silent = false)
    {
        foreach (Entrance entrance in unconnectedEntrances)
        {
            entrance.CloseDoor();
        }
        if (!silent) audioScource.PlayOneShot(closeDoorAudio);
    }


    public void OpenDoors(bool silent = false)
    {
        foreach (Entrance entrance in unconnectedEntrances)
        {
            Debug.Log("manager closing door");
            entrance.OpenDoor();
        }
        if (!silent) audioScource.PlayOneShot(openDoorAudio);
    }
}