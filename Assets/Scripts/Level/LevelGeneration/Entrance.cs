using System;
using UnityEditor.EditorTools;
using UnityEngine;

public class Entrance : MonoBehaviour
{
    public Direction direction;
    [NonSerialized] public GameObject roomToSpawn;

    protected bool doorOpen = true;

    RoomManager roomManager;
    GameObject doorCollisionObject;

    private void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
        doorCollisionObject = transform.GetChild(0).gameObject;

        if (roomManager != null ) 
            roomManager.entrances.Add(this);

        doorCollisionObject.SetActive(doorOpen);
    }


    public void SpawnDoorCover()
    {
        roomManager.SpawnDoorCover(direction, transform.position);
        Die();
    }

    public void Die()
    {
        if (roomManager.entrances.Contains(this))
            roomManager.entrances.Remove(this);

        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    protected void CloseDoor()
    {
        if (doorOpen) ToggleDoor();
    }

    protected void OpenDoor()
    {
        if (!doorOpen) ToggleDoor();
    }

    protected void ToggleDoor()
    {
        doorOpen = !doorOpen;

        doorCollisionObject.SetActive(doorOpen);
    }
}
