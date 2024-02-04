using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Bottom,
    Left,
    Top, 
    Right
}

public class Room : MonoBehaviour
{
    public List<Direction> roomOpeningDirections;


    // Cached references
    RoomManager roomManager;
    Transform parent;

    void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
        parent = GetComponentInParent<Transform>();

        Invoke("SpawnRooms", 3f);
    }

    void SpawnRooms()
    {
        foreach(Direction direction in roomOpeningDirections) 
        {
            //Invert direction to get the opening direction the new room needs
            Direction invertedDirection = InvertDirection(direction);

            float xPos = direction == Direction.Left ? -roomManager.roomWidthInUnits :
                         direction == Direction.Right ? roomManager.roomWidthInUnits : 0;
            float yPos = direction == Direction.Bottom ? -roomManager.roomHeightInUnits :
                         direction == Direction.Top ? roomManager.roomHeightInUnits : 0;

            GameObject roomSpawner = Instantiate(roomManager.RoomSpawnerPrefab, new Vector2(xPos+parent.position.x, yPos + parent.position.y), Quaternion.identity, transform);
            roomSpawner.GetComponent<RoomSpawner>().openingDirection.Add(invertedDirection);
        }
    }


    Direction InvertDirection(Direction originalDirection)
    {
        int enumLength = Enum.GetValues(typeof(Direction)).Length;
        int halfEnumLength = enumLength / 2;

        int originalValue = (int)originalDirection;
        int invertedValue = (originalValue + halfEnumLength) % enumLength;

        return (Direction)invertedValue;
    }
}
