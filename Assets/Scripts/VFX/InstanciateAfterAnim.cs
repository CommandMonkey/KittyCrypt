using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InstanciateAfterAnim : MonoBehaviour
{
    [SerializeField] GameObject content;
    private EncounterRoom encounterRoom; // Reference to the EncounterRoom script

    // Method to initialize the InstanciateAfterAnim script with a reference to the EncounterRoom
    public void Initialize(EncounterRoom room)
    {
        encounterRoom = room;
    }

    public void OnAnimDone()
    {
        GameObject instance = Instantiate(content, transform.position, Quaternion.identity);

        // Register the enemy to the EncounterRoom enemies list
        if (encounterRoom != null)
        {
            encounterRoom.RegisterEnemy(instance);
        }


        Destroy(gameObject);
    }
}
