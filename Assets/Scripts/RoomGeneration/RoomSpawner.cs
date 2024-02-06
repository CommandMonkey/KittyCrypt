using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.EditorTools;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    [SerializeField] List<Direction> openingDirection = new List<Direction>();

    private static List<Vector3> spawnPositions = new List<Vector3>();
    private static Dictionary<Vector3, RoomSpawner> survivingInstances = new Dictionary<Vector3, RoomSpawner>();

    Transform parentTransform;
    RoomManager roomManager;

    private void Start()
    {
        Vector3 spawnPos = transform.position;
        Debug.Log(spawnPositions.Count);

        if (spawnPositions.Contains(spawnPos))
        {
            if (survivingInstances.TryGetValue(spawnPos, out RoomSpawner survivingInstance))
            {
                survivingInstance.AddOpeningDirection(openingDirection[0]);
            }

            // DIE
        }
        else
        {
            spawnPositions.Add(spawnPos);
            Debug.Log("Live");
        }
    }

    public void AddOpeningDirection(Direction direction)
    {
        openingDirection.Add(direction);
    }


    void SpawnRoom()
    {
        

        Debug.Log(openingDirection);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Room"))
        {
            Die();
        }
        //else if (other.CompareTag("RoomSpawner"))
        //{
        //    RoomSpawner otherSpawner = other.GetComponent<RoomSpawner>();
        //    if (otherSpawner.initiated)
        //    {
        //        otherSpawner.openingDirection.Add(openingDirection[0]);
        //        initiated = false;
        //        Die();
        //    }
        //}
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
    }
}