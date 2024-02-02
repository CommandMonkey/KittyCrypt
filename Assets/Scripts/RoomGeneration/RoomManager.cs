using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] int LevelWidth = 16;
    [SerializeField] int LevelHeight = 16;
    [Header("Rooms")]
    [SerializeField] float roomWidthInUnits = 16;
    [SerializeField] float roomHeightInUnits = 16;
    [SerializeField] GameObject[] rooms;

    GameObject[,] roomsArray;

    void Start()
    {
        
    }

    void StartRoomGeneration()
    {
        GenerateRoomArray();

    }

    void GenerateRoomArray()
    {
        roomsArray = new GameObject[LevelWidth, LevelHeight];

        for (int x = 0; x < LevelWidth; x++)
        {
            for (int y = 0; y < LevelHeight; y++)
            {
                roomsArray[x, y] = rooms[0];
            }
        }
    }

    Vector2 _roomPos;
    void SpawnRooms()
    {
        for (int _x = 0; _x < LevelWidth; _x++)
        {
            for (int _y = 0; _y < LevelHeight; _y++)
            {
                _roomPos = new Vector2(_x * roomWidthInUnits, _y * roomHeightInUnits);

                Instantiate(roomsArray[_x, _y], _roomPos, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
