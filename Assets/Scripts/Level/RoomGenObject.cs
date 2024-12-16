using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RoomGenData", menuName = "ScriptableObjects/RoomGenData")]
public class RoomGenObject : ScriptableObject
{
#region Fields

    [Header("Room Prefabs")]
    public GameObject startRoom;
    [SerializeField] List<RoomProbability> randomRoomProbabilities;
    [SerializeField] int amountOfRandomRooms;
    [SerializeField] List<GameObject> setRooms;

    [Tooltip("spawned furthest away from each other, in order of list")] 
    public List<GameObject> endRooms = null;

    [Header("EntranceBlockers")]
    public List<DirectionGameObjectPair> entranceBlockers;

    private Dictionary<GameObject, int> spawnedRoomCounts = new Dictionary<GameObject, int>();
    
#endregion

#region Methods
    
    // Returns a randomized list of all the rooms that should be spawned
    public List<GameObject> GetRoomsList()
    {
        List<GameObject> resultList = new List<GameObject>();
        foreach (GameObject room in setRooms)
        {
            resultList.Add(room);
        }

        if (amountOfRandomRooms > 0)
        {
            for (int i = 0; i < amountOfRandomRooms; i++)
            {
                GameObject randomRoom = ChooseRandomRoom();
                if (randomRoom != null)
                {
                    resultList.Add(randomRoom);
                }
            }
        }

        foreach (RoomProbability roomProbability in randomRoomProbabilities)
            roomProbability.Reset();

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

    
    private GameObject ChooseRandomRoom()
    {
        float totalProbability = 0f;
        foreach (var roomProb in randomRoomProbabilities)
        {
            totalProbability += roomProb.spawnProbability;
        }

        float randomValue = UnityEngine.Random.Range(0f, totalProbability);
        float cumulativeProbability = 0f;

        foreach (var roomProb in randomRoomProbabilities)
        {
            cumulativeProbability += roomProb.spawnProbability;
            if (randomValue <= cumulativeProbability && roomProb.currentAmount < roomProb.maxAmount)
            {
                roomProb.currentAmount++;
                return roomProb.roomPrefab;
            }
        }

        return null;
    }
    
#endregion
}


[System.Serializable]
public class RoomProbability
{
#region Fields

    public GameObject roomPrefab;
    [Range(0f, 1f)] public float spawnProbability = 0.5f;
    public int maxAmount = 1; 
    [NonSerialized] public int currentAmount = 0;

    public void Reset()
    {
        currentAmount = 0;
    }
    
#endregion
}


[System.Serializable]
public class DirectionGameObjectPair
{
#region Fields

    public Direction direction;
    public GameObject gameObject;
    

    #endregion
}
