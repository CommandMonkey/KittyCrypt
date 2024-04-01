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
    [NonSerialized] internal UnityEvent OnSpawnRoomDone;
    [NonSerialized] internal List<Entrance> entrances = new List<Entrance>();

    // private fields
    List<GameObject> roomsToSpawn = new List<GameObject>();
    List<Room> spawnedRooms = new List<Room>();

    // Cached refs
    LevelManager levelManager;
    AudioSource audioScource;

    void Start()
    {
        OnSpawnRoomDone = new UnityEvent();
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
        // Define the contact filter for room colliders
        ContactFilter2D roomsFilter = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Room"),
            useTriggers = true
        };

        int previousRoomCount = 0;
        int waveCount = 0;

        while (roomsToSpawn.Count > 0)
        {
            waveCount++;

            // Check termination condition
            if (waveCount > 10 && roomsToSpawn.Count == previousRoomCount)
            {
                LogWarningPerRooms("TERMINATING ROOM SPAWNING, can't spawn room: ");
                DestroyAllRooms();
                StartRoomSpawning();
                yield break;
            }
            previousRoomCount = roomsToSpawn.Count;

            // Shuffle the list of rooms to spawn
            List<GameObject> shuffledRooms = GameHelper.ShuffleList(roomsToSpawn);
            yield return new WaitForEndOfFrame();

            // Iterate over entrances
            foreach (Entrance entrance in GameHelper.ShuffleList(entrances))
            {
                Direction invDirection = InvertDirection(entrance.direction);
                Vector3 entrancePosition = entrance.gameObject.transform.position;

                // Check if the entrance has a connected room
                if (!entrance.hasConnectedRoom)
                {
                    foreach (GameObject roomObj in shuffledRooms)
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
                                shuffledRooms.Remove(roomObj);
                                roomsToSpawn.Remove(roomObj);
                                break;
                            }
                        }
                    }
                    yield return new WaitForEndOfFrame();


                }


            }


        }
        // Get entrance that is furthest away
        Entrance furthestAway = null;
        float furthestAwayDistance = 0;
        foreach (Entrance entr in entrances)
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
        if (furthestAway != null)
        {
            // spawn endroom
            Room endRoomComponent = levelData.endRoom.GetComponent<Room>();
            Entrance endRoomEntrance = GetEntranceOfDir(endRoomComponent, InvertDirection(furthestAway.direction));

            Vector3 endEentranceToZero = Vector3.zero - endRoomEntrance.transform.localPosition;
            Vector3 endSpawnPosition = furthestAway.transform.position + endEentranceToZero;
            SpawnRoom(levelData.endRoom, endSpawnPosition, furthestAway);
        }

        yield return new WaitForEndOfFrame();

        // Spawn door covers for entrances without connected rooms
        foreach (Entrance entrance in entrances)
        {
            if (!entrance.hasConnectedRoom)
                entrance.SpawnDoorCover();
        }

        levelManager.state = LevelManager.LevelState.Running;
        CloseDoors(true);
        OnSpawnRoomDone.Invoke();
    }


    // /////// ================= /////// //
    // ///////  HELPER FUNCTION  /////// //
    // /////// ================= /////// //

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
            Debug.Log("manager closing door");
            entrance.OpenDoor();
        }
        if (!silent) audioScource.PlayOneShot(openDoorAudio);
    }
}