using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.EditorTools;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public List<Direction> openingDirections = new List<Direction>();
    public RoomType roomType = RoomType.Normal;

    private static List<Vector3> spawnPositions = new List<Vector3>();
    private static Dictionary<Vector3, RoomSpawner> survivingInstances = new Dictionary<Vector3, RoomSpawner>();
    Vector3 spawnPos;
    int wave;

    Transform parentTransform;
    RoomManager roomManager;
    Collider2D collider;

    private void Start()
    {
        // Init References
        parentTransform = FindObjectOfType<Grid>().transform;
        roomManager = FindObjectOfType<RoomManager>();
        collider = GetComponent<BoxCollider2D>();

        // subscribe to spawn room Event
        roomManager.OnSpawnRooms.AddListener(SpawnRoom);
        roomManager.OnNewSpawnWave.AddListener(NewSpawnWave);

        spawnPos = transform.TransformPoint(Vector3.right);
        wave = roomManager.currentWave;

    }

    void NewSpawnWave()
    {
        if (spawnPositions.Contains(spawnPos))
        {
            if (survivingInstances.TryGetValue(spawnPos, out RoomSpawner survivingInstance))
            {
                survivingInstance.AddOpeningDirection(openingDirections[0]);
            }
        }
        else
        {
            spawnPositions.Add(spawnPos);
            survivingInstances.Add(spawnPos, this);
            roomManager.RegisterRoom();
        }
    }

    public void AddOpeningDirection(Direction direction)
    {
        openingDirections.Add(direction);
    }


    void SpawnRoom()
    {
        GameObject room = roomManager.GetRandomRoom(openingDirections);
        Instantiate(room, transform.position, Quaternion.identity, parentTransform);
        //Debug.Log(room.name);
        Die();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("Room"))
        {
            Die();
        }
    }

    void Die()
    {   
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        // Clear the spawnPositions HashSet when the script is disabled
        spawnPositions.Clear();
        survivingInstances.Clear();
    }

    void SpawnRoomSpawners()
    {
        foreach (Direction _direction in openingDirections)
        {
            //Invert direction to get the opening direction the new room needs
            Direction _invertedDirection = InvertDirection(_direction);

            Vector3 _spawnerPosition = DirectionToVector(_direction) * roomManager.roomGridSizeInUnits;

            Instantiate(roomManager.RoomSpawnerPrefab, _spawnerPosition + parentTransform.position, Quaternion.identity, transform)
            .GetComponent<RoomSpawner>().AddOpeningDirection(_invertedDirection);
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