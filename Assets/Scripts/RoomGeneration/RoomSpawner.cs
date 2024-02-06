using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    [NonSerialized] public List<Direction> openingDirection = new List<Direction>();

    Transform parentTransform;

    private void Start()
    {
        parentTransform = GetComponentInParent<Transform>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Room"))
        {
            // if touching room then die
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else if (other.CompareTag("RoomSpawner"))
        {
            
        }
    }
}