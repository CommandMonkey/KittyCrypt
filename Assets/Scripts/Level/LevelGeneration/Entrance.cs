using System;
using UnityEngine;

public class Entrance : MonoBehaviour
{
    public Direction direction;
    [NonSerialized] public GameObject roomToSpawn;

    protected bool _doorOpen = true;

    RoomManager roomManager;
    GameObject doorCollisionObject;

    private void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
        doorCollisionObject = transform.GetChild(0).gameObject;

        if (roomManager != null ) 
            roomManager.entrances.Add(this);

        doorCollisionObject.SetActive(_doorOpen);
    }


    public void SpawnDoorCover()
    {
        roomManager.SpawnDoorCover(direction, transform.position);
        Die();
    }

    public void Die()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    protected void CloseDoor()
    {
        if (_doorOpen) ToggleDoor();
    }

    protected void OpenDoor()
    {
        if (!_doorOpen) ToggleDoor();
    }

    protected void ToggleDoor()
    {
        _doorOpen = !_doorOpen;

        doorCollisionObject.SetActive(_doorOpen);
    }
}
