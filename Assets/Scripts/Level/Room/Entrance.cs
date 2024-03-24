using System;
using System.Collections.Generic;
using UnityEngine;


public class Entrance : MonoBehaviour
{
    public Direction direction;

    [NonSerialized] public GameObject roomToSpawn;
    [NonSerialized] public List<string> roomTriesNames = new List<string>();
     public bool hasConnectedRoom;

    bool doorOpen = true;

    RoomManager roomManager;
    GameObject doorCollisionObject;
    Animator animator;

    private void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
        doorCollisionObject = transform.GetChild(0).gameObject;
        animator = GetComponent<Animator>();

        animator.SetInteger("direction", (int)direction);

        SetDoorCollisionSize();

        if (roomManager != null ) 
            roomManager.entrances.Add(this);

        doorCollisionObject.SetActive(!doorOpen);

    }

    private void SetDoorCollisionSize()
    {
        int numDir = (int)direction;
        doorCollisionObject.GetComponent<BoxCollider2D>().size =
            numDir == 0 || numDir == 2 ? new Vector2(3, 1) : new Vector2(1, 3);
    }

    public void SpawnDoorCover()
    {
        roomManager.SpawnDoorCover(direction, transform.position);
        Invoke("Die", .1f);
    }

    public void Die()
    {
        if (roomManager.entrances.Contains(this))
            roomManager.entrances.Remove(this);

        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    internal void CloseDoor()
    {
        if (doorOpen) ToggleDoor();
    }

    internal void OpenDoor()
    {
        if (!doorOpen) ToggleDoor();
    }

    protected void ToggleDoor()
    {
        doorOpen = !doorOpen;

        doorCollisionObject.SetActive(!doorOpen);
        animator.SetBool("doorOpen", doorOpen);
    }


}
