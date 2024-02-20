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

public enum RoomType
{
    Start, 
    End,
    Normal
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

    }




}
