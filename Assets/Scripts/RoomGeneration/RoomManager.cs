using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] int LevelWidth = 16;
    [SerializeField] int LevelHeight = 16;
    [Header("Rooms")]
    public Transform grid;
    public float roomWidthInUnits = 16;
    public float roomHeightInUnits = 16;
    [Header("Room Spawning Prefabs")]
    public GameObject RoomSpawnerPrefab;
    [SerializeField] List<GameObject> BottomOpeningrooms;
    [SerializeField] List<GameObject> LeftOpeningrooms;
    [SerializeField] List<GameObject> TopOpeningrooms;
    [SerializeField] List<GameObject> RightOpeningrooms;

    GameObject[,] roomsArray;

    void Start()
    {
        StartRoomGeneration();
    }

    /// <summary>
    /// hhejwhfsehf
    /// </summary>
    void StartRoomGeneration()
    {

    }



    GameObject _room;
    int _rand;
    /// <summary>
    /// returns a random room based on the opening direction needed
    /// </summary>
    public GameObject GetRandomRoom(Direction direction)
    {
        List<GameObject> _list = GetListFromDirecton(direction);
        _rand = UnityEngine.Random.Range(0, BottomOpeningrooms.Count - 1);
        _room = BottomOpeningrooms[_rand];
        return _room;
    }

    public GameObject GetRandomRoom(Direction[] directions)
    {
        List<GameObject> _validRooms = GetListFromDirecton(directions[0]).ToList();
        for (int i = 1; i < directions.Length; i++) 
        {
            for(int j = 0; j > _validRooms.Count; j++)
            {
                if (RoomHasDirection(_validRooms[i], directions[i]))
                    _validRooms.RemoveAt(i);
            }
        }
        return _validRooms[UnityEngine.Random.Range(0, _validRooms.Count - 1)];
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


    // Update is called once per frame
    void Update()
    {
        
    }
}
