using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData")]
public class LevelDataObject : ScriptableObject
{
    [Header("Room Prefabs")]
    public GameObject startRoom;
    public List<GameObject> rooms;
    public GameObject endRoom;
    [Header("EntranceBlockers")]
    public List<DirectionGameObjectPair> entranceBlockers;

    public GameObject GetEntranceBlockerOfDir(Direction dir)
    {
        foreach (DirectionGameObjectPair pair in entranceBlockers)
        {
            if (pair.direction == dir)
                return pair.gameObject;
        }
        return null;
    }
}


[System.Serializable]
public class DirectionGameObjectPair
{
    public Direction direction;
    public GameObject gameObject;
}
