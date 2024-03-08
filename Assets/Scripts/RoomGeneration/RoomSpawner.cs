using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public List<Direction> openingDirections = new List<Direction>();
    public RoomType roomType = RoomType.Normal;
    [NonSerialized] public bool spawnedChildren = false;

    private static Dictionary<Vector3, RoomSpawner> survivingInstances = new Dictionary<Vector3, RoomSpawner>();
    Vector3 spawnPos;
    

    Transform parentTransform;
    RoomManager roomManager;

    private void Start()
    {
        // Init References
        parentTransform = FindObjectOfType<Grid>().transform;
        roomManager = FindObjectOfType<RoomManager>();

        // Subscribe to spawn room Event
        //roomManager.OnSpawnRooms.AddListener(SpawnRoom);
        //roomManager.OnNewSpawnWave.AddListener(NewSpawnWave);

        //roomManager.RegisterSpawner();

        spawnPos = transform.TransformPoint(Vector3.right);

        if (survivingInstances.TryGetValue(spawnPos, out RoomSpawner survivingInstance))
        {
            survivingInstance.AddOpeningDirection(openingDirections[0]);
            Die();
        }
        else
        {
            survivingInstances.Add(spawnPos, this);
            //roomManager.RegisterSpawner();
        }   

    }

    void NewSpawnWave()
    {
        if (spawnedChildren) return;
        spawnedChildren = true;
        SpawnRoomSpawners();
    }

    public void AddOpeningDirection(Direction direction)
    {
        if (!openingDirections.Contains(direction))
            openingDirections.Add(direction);
    }


    void SpawnRoom()
    {
        //GameObject room = roomManager.GetRandomRoom(openingDirections);
        //Instantiate(room, transform.position, Quaternion.identity, parentTransform);
        //Debug.Log(room.name);
        Die();
    }

    void Die()
    {   
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        // Clear the static SurvivingInstances Dictionary when the script is disabled
        survivingInstances.Clear();
    }

    void SpawnRoomSpawners()
    {

        // Spawn children spawners
        for (int j = 0; j < openingDirections.Count; j++)
        {
            //Invert direction to get the opening direction the new room needs
            List<Direction> _instanceDirections = new List<Direction>();
            _instanceDirections.Add(InvertDirection(openingDirections[j]));


                
            int _extraDirections = UnityEngine.Random.Range(1, 2);
            // add random directions
            for (int i = 0; i < _extraDirections;)
            {
                Direction _dir = (Direction)UnityEngine.Random.Range(0, 3);
                if (!_instanceDirections.Contains(_dir))
                {
                    _instanceDirections.Add(_dir);
                    i++;
                }

            }

            //Instantiate(roomManager.RoomSpawnerPrefab, _spawnerPosition, Quaternion.identity, transform)
            //.GetComponent<RoomSpawner>().openingDirections = _instanceDirections;
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
        float xPos = _direction == Direction.Left ? -1 :
                     _direction == Direction.Right ? 1 : 0;
        float yPos = _direction == Direction.Bottom ? -1 :
                     _direction == Direction.Top ? 1 : 0;
        return new Vector2(xPos, yPos);
    }
}