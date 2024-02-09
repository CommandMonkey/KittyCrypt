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

        SpawnRoomSpawners();
    }

    void SpawnRoomSpawners()
    {
        foreach(Direction _direction in roomOpeningDirections) 
        {
            //Invert direction to get the opening direction the new room needs
            Direction _invertedDirection = InvertDirection(_direction);

            Vector3 _spawnerPosition = DirectionToVector(_direction) * roomManager.roomGridSizeInUnits;

            GameObject roomSpawner = Instantiate(roomManager.RoomSpawnerPrefab, _spawnerPosition + parent.position, Quaternion.identity, transform);
            roomSpawner.GetComponent<RoomSpawner>().AddOpeningDirection(_invertedDirection);
        }
    }


    Direction InvertDirection(Direction _originalDirection)
    {
        int enumLength = Enum.GetValues(typeof(Direction)).Length;
        int halfEnumLength = enumLength / 2;

        int originalValue = (int)_originalDirection;
        int invertedValue = (originalValue + halfEnumLength) % enumLength;

        return (Direction)invertedValue;
    }

    Vector2 DirectionToVector(Direction _direction)
    {
        float xPos = _direction == Direction.Left    ? -1 :
                     _direction == Direction.Right   ?  1 : 0;
        float yPos = _direction == Direction.Bottom  ? -1 :
                     _direction == Direction.Top     ?  1 : 0;
        return new Vector2(xPos, yPos);
    }
}
