using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData")]
public class LevelDataObject : ScriptableObject
{
    [Header("Room Prefabs")]
    public GameObject startRoom;
    [SerializeField] List<GameObject> randomRoomPool;
    [SerializeField] int amountOfRandomRooms;
    [SerializeField] List<GameObject> setRooms;
    public GameObject endRoom = null;
    [Header("EntranceBlockers")]
    public List<DirectionGameObjectPair> entranceBlockers;


    public List<GameObject> GetRoomsList()
    {
        List<GameObject>  resultList = new List<GameObject>();
        foreach(GameObject room in setRooms)
        {
           resultList.Add(room);
        }
        if (amountOfRandomRooms > 0) 
        { 
            for (int i = 0; i <= amountOfRandomRooms; i++)
            {
                resultList.Add(randomRoomPool[Random.Range(0, randomRoomPool.Count)]);
            }
        }
        
        return resultList;
    }


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
