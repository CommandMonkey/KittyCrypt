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
    [SerializeField] List<Direction> startRoomDirections;
    

    [Header("Room Spawning Prefabs")]
    public GameObject RoomSpawnerPrefab;
    [SerializeField] List<GameObject> BottomOpeningrooms;
    [SerializeField] List<GameObject> LeftOpeningrooms;
    [SerializeField] List<GameObject> TopOpeningrooms;
    [SerializeField] List<GameObject> RightOpeningrooms;

    [NonSerialized] public UnityEvent OnSpawnRooms;
    [NonSerialized] public UnityEvent OnNewSpawnWave;
    [NonSerialized] public int amountOfSpawners;

    int spawnedRooms = 1;
    GameObject[,] roomsArray;

    void Start()
    {
        OnSpawnRooms = new UnityEvent();
        OnNewSpawnWave = new UnityEvent();
        StartRoomGeneration();
    }


    void StartRoomGeneration()
    {
        RoomSpawner roomSpawner = Instantiate(RoomSpawnerPrefab, Vector3.zero, Quaternion.identity, grid)
            .GetComponent<RoomSpawner>();

        roomSpawner.roomType = RoomType.Start;
        roomSpawner.openingDirections = startRoomDirections;

        StartCoroutine(InvokeNewSpawnerWaveRoutine());
    }

    IEnumerator InvokeNewSpawnerWaveRoutine()
    {
        for(int i = 0; i < 4; i++) // DEBUG
        {
            yield return new WaitForSeconds(roomSpawningDelay);
            OnNewSpawnWave.Invoke();
        }
        yield return new WaitForSeconds(roomSpawningDelay);
        OnSpawnRooms.Invoke();
    }


    GameObject _room;
    int _rand;
    /// <summary>
    /// returns a random room based on the opening direction needed
    /// </summary>
    public GameObject GetRandomRoom(List<Direction> directions)
    {
        List<GameObject> _validRooms = GetListFromDirecton(directions[0]).ToList();
        for (int i = 1; i < directions.Count; i++) 
        {
            for(int j = 0; j > _validRooms.Count; j++)
            {
                if (RoomHasDirection(_validRooms[i], directions[i]))
                    _validRooms.RemoveAt(i);
            }
        }

        _room = _validRooms[UnityEngine.Random.Range(0, _validRooms.Count - 1)];
        if (_room.GetComponent<Room>().roomOpeningDirections.Count < maxRooms - spawnedRooms)
            return _room;
        else 
            return GetRandomRoom(directions);
    }

    private bool RoomHasDirection(GameObject room, Direction direction)
    {
        return room.GetComponent<Room>().roomOpeningDirections.Contains(direction);
    }


    List<GameObject> GetListFromDirecton(Direction direction) 
    {
        switch (direction)
        {
            case (Direction.Bottom):   return BottomOpeningrooms;
            case (Direction.Left):     return LeftOpeningrooms;
            case (Direction.Top):      return TopOpeningrooms;
            case (Direction.Right):    return RightOpeningrooms;
        }
        return null;
    }

    public void RegisterSpawner()
    {
        amountOfSpawners++;
        Debug.Log(amountOfSpawners);
    }
}
